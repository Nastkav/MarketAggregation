
using AngleSharp.Html.Parser;
using AngleSharp;
using MyShop.Models;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MyShop.Helpers
{
    public class MetroHelper: IShop
    {
        public string SearchProducts_link { get; set; }
        public string SaleProducts_link { get; set; }
        public string AllProductsInTheCategory_link { get; set; }
        public MetroHelper()
        {
            this.SearchProducts_link = "https://metro.zakaz.ua/uk/search/?q=";
            this.SaleProducts_link = "https://metro.zakaz.ua/uk/promotions/";
            this.AllProductsInTheCategory_link = "https://metro.zakaz.ua/uk/categories/";
        }
        public async Task<List<ProductItem>> RecommendedProducts(string category)
        {
            string rec_cat = this.AllProductsInTheCategory_link + category;
            List<ProductItem> answer = new List<ProductItem>();
            try
            {
                var htmlString = await DownloadPage(rec_cat);
                answer = MarkupToPdroducts(htmlString, "recommended", 5);
            }
            catch (Exception e)
            {
                // logs
            }
            return answer;
        }
        private List<ProductItem> MarkupToPdroducts(string htmlString, string type, int? Take = null)
        {
            List<ProductItem> answer = new List<ProductItem>();
            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(htmlString);
            var list = document.QuerySelectorAll(".ProductsBox__listItem");
            int count = 0;
            foreach (var item in list)
            {
                if (Take != null && Take == count)
                {
                    continue;
                }
                var Name_ = item.QuerySelector(".ProductTile__title").TextContent;
                var Sale = item.QuerySelector(".Badge_isDiscount .Badge__text") != null ? item.QuerySelector(".Badge_isDiscount .Badge__text").TextContent : "";
                var OldPrice_ = item.QuerySelector(".Price__value_minor") != null ? item.QuerySelector(".Price__value_minor").TextContent : null;
                ProductItem pi = new ProductItem();
                pi.Owner = "metro";
                pi.Availability = item.QuerySelector(".ProductTile__unavailable") == null ? true : false;
                pi.Name = Name_ != null ? Name_.Trim() : "";
                pi.Sale = Sale != null ? Sale.Trim() : "";
                pi.ImageUrl = item.QuerySelector(".ProductTile__image").GetAttribute("src");
                if (Sale == "")
                {
                    pi.Price = item.QuerySelector(".Price__value_caption").TextContent;
                }
                else
                {
                    pi.Price = item.QuerySelector(".Price__value_discount").TextContent;
                }
                pi.OriginalLink = "https://metro.zakaz.ua" + item.QuerySelector(".ProductTile").GetAttribute("href");
                if (OldPrice_ != null)
                {
                    pi.OldPrice = OldPrice_;
                }
                else
                {
                    pi.OldPrice = null;
                }
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
                answer = MarkupToPdroducts(htmlString, "sale");
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
                answer = MarkupToPdroducts(htmlString, "recommended");
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
                answer = MarkupToPdroducts(htmlString, "recommended", 15);
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
            var product_ = document.QuerySelector(".BigProductCard__content");
            if (product_ != null)
            {
                var Availability_ = product_.QuerySelector(".AddButton .icon-basket") != null ? true : false;
                var Amount_ = product_.QuerySelector(".BigProductCardTopInfo__amount") != null ? product_.QuerySelector(".BigProductCardTopInfo__amount").TextContent : null;
                var Name_ = product_.QuerySelector(".BigProductCardTopInfo__title").TextContent;
                var Sale = product_.QuerySelector(".Badge_isDiscount .Badge__text") != null ? product_.QuerySelector(".Badge_isDiscount .Badge__text").TextContent : "";
                var OldPrice_ = product_.QuerySelector(".Price__value_minor") != null ? product_.QuerySelector(".Price__value_minor").TextContent : null;
                answer.Owner = "metro";
                answer.Name = Name_ != null ? Name_.Trim() : "";
                answer.Sale = Sale != null ? Sale.Trim() : "";
                answer.ImageUrl = product_.QuerySelector(".ZoomableImageSwitcher > img").GetAttribute("src");
                if (Sale == "")
                {
                    answer.Price = product_.QuerySelector(".Price__value_title").TextContent;
                }
                else
                {
                    answer.Price = product_.QuerySelector(".Price__value_discount").TextContent;
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