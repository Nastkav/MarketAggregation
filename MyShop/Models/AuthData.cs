using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class AuthUser
    {
        public string Login { get; set; }
        public bool IsAuth { get; set; }
        public List<FavoriteProducts> MyFavorites { get; set; }
}
}