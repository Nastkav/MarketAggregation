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
    public class ProductsController : Controller
    {
        public async Task<ActionResult> Index(string category)
        {
            string current_category = "---";
            try
            {
                var CurrentCategory_ = Extension.comparison.Where(x => x.Key == category).FirstOrDefault();
                current_category = CurrentCategory_.Value.Name;
            }
            catch (Exception e) { }
            ViewData["current_category"] = current_category;
            return View();
        }

        public ActionResult Search(string search_str)
        {
            return View("Search");
        }

        public ActionResult Detailed()
        {
            var CompareProducts = this.Session["CompareProducts"] as bool?;
            if (CompareProducts != null && CompareProducts == true)
            {
                this.Session["CompareProducts"] = false;
                return View("Detailed");
            }
            else
            {
                return RedirectToRoute(new { controller = "Home", action = "Index" });
            }
        }
    }
}