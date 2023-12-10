using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models
{
    interface IShop
    {
        string SaleProducts_link { get; set; }
        string AllProductsInTheCategory_link { get; set; }
        Task<List<ProductItem>> RecommendedProducts(string category);
        Task<List<ProductItem>> SaleProducts();
        Task<List<ProductItem>> AllProductsInTheCategory(string category);
        Task<ProductItem> GetProductInformation(string ProductLink);
    }
}
