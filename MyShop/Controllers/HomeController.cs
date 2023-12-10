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
    public class HomeController : Controller
    {

        public async Task<ActionResult> Index()
        {
            
            return View();
        }
    }
}