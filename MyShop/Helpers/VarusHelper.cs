using AngleSharp.Html.Parser;
using AngleSharp;
using MyShop.Models;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Diagnostics;

namespace MyShop.Helpers
{
    public class VarusHelper: IShop
    {
        public string SearchProducts_link { get; set; }
        public string SaleProducts_link { get; set; }
        public string AllProductsInTheCategory_link { get; set; }
        public VarusHelper() 
        {
            this.SearchProducts_link = "https://go.varus.ua/search?q=";

            this.SaleProducts_link = "https://go.varus.ua/rasprodazha";

            this.AllProductsInTheCategory_link = "https://go.varus.ua/";
        }

        public async Task<List<ProductItem>> RecommendedProducts(string category)
        {
            string rec_cat = this.AllProductsInTheCategory_link + category;
            List<ProductItem> answer = new List<ProductItem>();

            try
            {

                var htmlString = await DownloadPage(rec_cat);

                answer = MarkupToPdroducts(htmlString,5);
            }
            catch (Exception e)
            {
                // logs
            }
            return answer;
        }
        private List<ProductItem> MarkupToPdroducts(string htmlString, int? Take = null)
        {
            List<ProductItem> answer = new List<ProductItem>();

            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();

            var document = parser.ParseDocument(htmlString);

            var list = document.QuerySelectorAll(".sf-product-card");
            int count = 0;
            foreach (var item in list)
            {

                if (Take != null && Take == count)
                {
                    continue;
                }

                var Name_ = item.QuerySelector(".sf-product-card__title").TextContent;
                var Sale = item.QuerySelector(".sf-product-card__badge > span") != null ? item.QuerySelector(".sf-product-card__badge > span").TextContent : "";
                var OldPrice_ = item.QuerySelector(".sf-price__old") != null ? item.QuerySelector(".sf-price__old").TextContent : null;
                var Price_ = item.QuerySelector(".sf-price__special") != null ? item.QuerySelector(".sf-price__special").TextContent.Split(' ')[0].Trim() : (item.QuerySelector(".sf-price__regular") != null ? item.QuerySelector(".sf-price__regular").TextContent.Split(' ')[0].Trim() : null);
                ProductItem pi = new ProductItem();
                pi.Owner = "varus";
                pi.Availability = item.QuerySelector(".sf-product-card__out-of-stock") == null ? true : false;
                pi.Name = Name_ != null ? Name_.Trim() : "";
                pi.Sale = Sale != null ? Sale.Trim() : "";
                pi.ImageUrl = item.QuerySelector(".sf-product-card__image img").GetAttribute("src");
                if (Price_ != null)
                {
                    pi.Price = Price_;
                }
                else
                {
                    pi.Price = null;
                }
                if (OldPrice_ != null)
                {
                    pi.OldPrice = OldPrice_;
                }
                else
                {
                    pi.OldPrice = null;
                }
                pi.OriginalLink = "https://go.varus.ua" + item.QuerySelector(".sf-product-card__link").GetAttribute("href");
                answer.Add(pi);
                count++;
            }
            return answer;
        }
        public async Task<List<ProductItem>> SaleProducts()
        {
            List<ProductItem> answer = new List<ProductItem>();
            try
            {
                var htmlString = await DownloadPage(this.SaleProducts_link);
                answer = MarkupToPdroducts(htmlString);
            }
            catch (Exception e)
            {
                // logs
            }
            return answer;
        }
        private async Task<string> DownloadPage(string link)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = path + @"\App_Data\PUP\chrome.exe",
            });

            var page = await browser.NewPageAsync();
            page.DefaultTimeout = 0; 
            await page.GoToAsync(link, WaitUntilNavigation.Networkidle2);

            await page.EvaluateFunctionAsync(@"async () => {
                await new Promise((resolve, reject) => {
                    var totalHeight = 0;
                    var distance = 100;
                    var timer = setInterval(() => {
                        var scrollHeight = document.body.scrollHeight;
                        window.scrollBy(0, distance);
                        totalHeight += distance;
                        if(totalHeight >= scrollHeight){
                            clearInterval(timer);
                            resolve();
                        }
                    }, 100);
                });
            }");

            string Content = await page.GetContentAsync();
            await page.CloseAsync();
            await browser.CloseAsync();
            return Content;
        }
        public async Task<List<ProductItem>> AllProductsInTheCategory(string category)
        {
            string rec_cat = this.AllProductsInTheCategory_link + category;
            List<ProductItem> answer = new List<ProductItem>();
            try
            {
                var htmlString = await DownloadPage(rec_cat);
                answer = MarkupToPdroducts(htmlString);
            }
            catch (Exception e)
            {
                // logs
            }
            return answer;
        }
        public async Task<List<ProductItem>> SearchProducts(string search)
        {
            string rec_cat = this.SearchProducts_link + search;
            List<ProductItem> answer = new List<ProductItem>();
            try
            {
                var htmlString = await DownloadPage(rec_cat);
                answer = MarkupToPdroducts(htmlString, 15);
            }
            catch (Exception e)
            {
                // logs
            }
            return answer;
        }
        public async Task<ProductItem> GetProductInformation(string ProductLink)
        {
            if (ProductLink == "")
            {
                return null;
            }
            ProductItem answer = new ProductItem();
            try
            {
                var htmlString = await DownloadPage(ProductLink);
                answer = MarkupToOnePdroduct(htmlString, ProductLink);
            }
            catch (Exception e)
            {

            }
            return answer;
        }
        private ProductItem MarkupToOnePdroduct(string htmlString, string link)
        {
            ProductItem answer = new ProductItem();
            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(htmlString);
            var product_ = document.QuerySelector(".o-product-details");
            if (product_ != null)
            {
                var Availability_ = product_.QuerySelector(".wrap .btn-not-available") == null ? true : false;
                var Amount_ = product_.QuerySelector(".price .count") != null ? product_.QuerySelector(".price .count").TextContent : null;
                var Name_ = product_.QuerySelector(".sf-heading__title").TextContent;
                var Sale = product_.QuerySelector(".sale-block > span") != null ? product_.QuerySelector(".sale-block > span").TextContent : "";
                var OldPrice_ = product_.QuerySelector(".sf-price__old") != null ? product_.QuerySelector(".sf-price__old").TextContent : null;
                var Price_ = product_.QuerySelector(".sf-price__special") != null ? product_.QuerySelector(".sf-price__special").TextContent.Split(' ')[0].Trim() : (product_.QuerySelector(".sf-price__regular") != null ? product_.QuerySelector(".sf-price__regular").TextContent.Split(' ')[0].Trim() : null);
                answer.Owner = "varus";
                answer.Name = Name_ != null ? Name_.Trim() : "";
                answer.Sale = Sale != null ? Sale.Trim() : "";
                answer.ImageUrl = product_.QuerySelector(".sf-image picture img").GetAttribute("src");
                if (Price_ != null)
                {
                    answer.Price = Price_;
                }
                else
                {
                    answer.Price = null;
                }
                if (OldPrice_ != null)
                {
                    answer.OldPrice = OldPrice_;
                }
                else
                {
                    answer.OldPrice = null;
                }
                answer.OriginalLink = link;
                answer.Amount = Amount_;
                answer.Availability = Availability_;
            }
            return answer;
        }
    }
}