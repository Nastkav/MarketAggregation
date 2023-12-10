using MyShop.Helpers;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyShop.Controllers
{
    public class MyofficeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            var UserInfo = this.Session["AuthUser"] as AuthUser;
            if (UserInfo != null)
            {
                return View();
            }
            else
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
        }
    }
}