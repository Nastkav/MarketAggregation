using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class ProductItem
    {
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string FullDescription { get; set; }
        public string Amount { get; set; }
        public bool? Availability { get; set; }
        public string Sale { get; set; }
        public string Owner { get; set; }
        public string Price { get; set; }
        public string OldPrice { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalLink { get; set; }
        public bool IsFavorites { get; set; }
    }
}