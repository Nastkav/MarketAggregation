using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class Product
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Amount { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalLink { get; set; }
        public bool IsFavorites { get; set; }
    }
}