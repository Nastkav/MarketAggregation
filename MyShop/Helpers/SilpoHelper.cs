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

namespace MyShop.Helpers
{
    public class SilpoHelper: IShop
    {
        public string SearchProducts_link { get; set; }
        public string SaleProducts_link { get; set; }
        public string AllProductsInTheCategory_link { get; set; }
        public SilpoHelper()
        {
            this.SearchProducts_link = "https://shop.silpo.ua/search/all?find=";
            this.SaleProducts_link = "https://shop.silpo.ua/all-offers";
            this.AllProductsInTheCategory_link = "https://shop.silpo.ua/category/";
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
            var list = document.QuerySelectorAll(".product-list-item-wrapper");
            int count = 0;
            foreach (var item in list)
            {
                if (Take != null && Take == count)
                {
                    continue;
                }
                var PriceText = item.QuerySelector(".price-text") != null ? item.QuerySelector(".price-text").TextContent.Trim() : "";
                var Name_ = item.QuerySelector(".product-title").TextContent;
                var Sale = item.QuerySelector(".old-price-discount") != null ? item.QuerySelector(".old-price-discount").TextContent : "";
                var OldPrice_ = item.QuerySelector(".old-integer") != null ? item.QuerySelector(".old-integer").TextContent : null;
                var Price_ = item.QuerySelector(".current-integer") != null ? item.QuerySelector(".current-integer").TextContent.Split(' ')[0].Trim() : null;
                ProductItem pi = new ProductItem();
                pi.Owner = "silpo";
                pi.Availability = item.QuerySelector(".add-to-comment-btn") != null && item.QuerySelector(".add-to-comment-btn").TextContent.Trim() == "Товар закінчився" ? false : true;
                pi.Name = Name_ != null ? Name_.Trim() : "";
                pi.Sale = Sale != null ? Sale.Trim() : "";
                pi.ImageUrl = item.QuerySelector(".image-content-wrapper img").GetAttribute("src");
                pi.Price = Price_;
                if (OldPrice_ != null)
                {
                    pi.OldPrice = OldPrice_;
                }
                else
                {
                    pi.OldPrice = null;
                }
                if (PriceText == "Роздріб")
                {
                    pi.OldPrice = item.QuerySelector(".current-integer").TextContent;
                    pi.Price = item.QuerySelector(".current-integer-opt").TextContent + $" ({PriceText})";
                }
                pi.OriginalLink = "https://shop.silpo.ua" + item.QuerySelector(".product-list-item__content").GetAttribute("href");
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
            //var browserFetcher = new BrowserFetcher(new BrowserFetcherOptions
            //{
            //    Path = @"D:\PUPP"
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
            var product_ = document.QuerySelector(".product-preview");
            if (product_ != null)
            {
                var Availability_ = product_.QuerySelector(".product-not-available") == null ? true : false;
                var Amount_ = product_.QuerySelector(".product-weight") != null ? product_.QuerySelector(".product-weight").TextContent : null;
                var PriceText = product_.QuerySelector(".price-text") != null ? product_.QuerySelector(".price-text").TextContent.Trim() : "";
                var Name_ = product_.QuerySelector(".product-preview_info .title").TextContent;
                var Sale = product_.QuerySelector(".old-price-discount") != null ? product_.QuerySelector(".old-price-discount").TextContent : "";
                var OldPrice_ = product_.QuerySelector(".old-integer") != null ? product_.QuerySelector(".old-integer").TextContent : null;
                var Price_ = product_.QuerySelector(".current-integer") != null ? product_.QuerySelector(".current-integer").TextContent.Split(' ')[0].Trim() : null;
                answer.Owner = "silpo";
                answer.Name = Name_ != null ? Name_.Trim() : "";
                answer.Sale = Sale != null ? Sale.Trim() : "";
                answer.ImageUrl = product_.QuerySelector(".slide img").GetAttribute("src");
                answer.Price = Price_;
                if (OldPrice_ != null)
                {
                    answer.OldPrice = OldPrice_;
                }
                else
                {
                    answer.OldPrice = null;
                }
                if (PriceText == "Роздріб")
                {
                    answer.OldPrice = product_.QuerySelector(".current-integer").TextContent;
                    answer.Price = product_.QuerySelector(".current-integer-opt").TextContent + $" ({PriceText})";
                }
                answer.OriginalLink = link;
                answer.Amount = Amount_;
                answer.Availability = Availability_;
            }
            return answer;
        }
    }
}