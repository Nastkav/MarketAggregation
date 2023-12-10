
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PuppeteerSharp;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using AngleSharp;
using System.Net.Http.Headers;
using System.Globalization;
using System.Linq.Expressions;
using System.Web.UI;

namespace MyShop.Helpers
{
    public class AtbHelper: IShop
    {
        public string SearchProducts_link { get; set; }
        public string SaleProducts_link { get; set; }
        public string AllProductsInTheCategory_link { get; set; }
        public AtbHelper()
        {
            this.SearchProducts_link = "https://www.atbmarket.com/sch?query=";
            this.SaleProducts_link = "https://zakaz.atbmarket.com/catalog/economy";
            this.AllProductsInTheCategory_link = "https://zakaz.atbmarket.com/catalog/";
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
            //var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            //{
            //    Path = @"C:\inetpub\wwwroot"
            //});
            //await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            string path = AppDomain.CurrentDomain.BaseDirectory;
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                ExecutablePath = path + @"\App_Data\PUP\chrome.exe",
            });
            var page = await browser.NewPageAsync();
            page.DefaultTimeout = 0; // or you can set this as 0
            await page.GoToAsync(link, WaitUntilNavigation.Networkidle2);
            string Content = await page.GetContentAsync();
            await page.CloseAsync();
            await browser.CloseAsync();
            return Content;
        }
        private List<ProductItem> MarkupToPdroducts(string htmlString, int? Take = null)
        {
            List<ProductItem> answer = new List<ProductItem>();
            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(htmlString);
            var list = document.QuerySelectorAll("article.catalog-item");
            int count = 0;
            foreach (var item in list)
            {
                if (Take !=null && Take == count)
                {
                    continue;
                }
                var Name_ = item.QuerySelector(".catalog-item__title a") !=null ? item.QuerySelector(".catalog-item__title a").TextContent : "";
                var Sale = item.QuerySelector("[data-tippy-content=\"Акційна пропозиція! Встигніть придбати за приємними цінами\"]") != null ? item.QuerySelector("[data-tippy-content=\"Акційна пропозиція! Встигніть придбати за приємними цінами\"]").TextContent : "";
                var OldPrice_ = item.QuerySelector(".product-price__bottom") != null ? item.QuerySelector(".product-price__bottom").GetAttribute("value") : null;
                ProductItem pi = new ProductItem();
                pi.Owner = "atb";
                pi.Availability = item.QuerySelector(".catalog-item__ends--not-available") == null ? true : false;
                pi.Name = Name_ != null ? Name_.Trim() : "";
                pi.Sale = Sale != null ? Sale.Trim() : "";
                pi.ImageUrl = item.QuerySelector(".catalog-item__img").GetAttribute("src");
                pi.Price = item.QuerySelector(".product-price__top").GetAttribute("value");
                if (OldPrice_ != null)
                {
                    pi.OldPrice = OldPrice_;
                }
                else
                {
                    pi.OldPrice = null;
                }
                pi.OriginalLink = "https://zakaz.atbmarket.com" + item.QuerySelector(".catalog-item__photo-link").GetAttribute("href");
                answer.Add(pi);
                count++;
            }
            return answer;
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
            var product_ = document.QuerySelector(".product-about");
            if (product_ != null)
            {
                var Availability_ = product_.QuerySelector(".available-tag__text") != null ? (product_.QuerySelector(".available-tag__text").TextContent.Trim() == "Є в наявності" ? true : false) : false;
                var Amount_ = product_.QuerySelector(".product-price__unit") != null ? product_.QuerySelector(".product-price__unit").TextContent : null;
                var Name_ = product_.QuerySelector(".product-page__title").TextContent;
                var Sale = product_.QuerySelector("[data-tippy-content=\"Акційна пропозиція! Встигніть придбати за приємними цінами\"]") != null ? product_.QuerySelector("[data-tippy-content=\"Акційна пропозиція! Встигніть придбати за приємними цінами\"]").TextContent : "";
                var OldPrice_ = product_.QuerySelector(".product-price__bottom") != null ? product_.QuerySelector(".product-price__bottom").TextContent : null;
                answer.Owner = "atb";
                answer.Name = Name_ != null ? Name_.Trim() : "";
                answer.Sale = Sale != null ? Sale.Trim() : "";
                answer.ImageUrl = product_.QuerySelector(".cardproduct-tabs__item picture img").GetAttribute("src");
                answer.Price = product_.QuerySelector(".product-price__top").TextContent;
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