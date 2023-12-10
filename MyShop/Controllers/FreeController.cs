using MyShop.Helpers;
using MyShop.Models;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;

namespace MyShop.Controllers
{
    public class FreeController : ApiController
    {
        [System.Web.Http.HttpGet]
        public async Task<IHttpActionResult> GetCompareProducts()
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var CompareProductsItems = session != null ? session["CompareProductsItems"] as CompareProducts : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                dynamic response = new ExpandoObject();
                try
                {
                    AtbHelper atb_h = new AtbHelper();
                    VarusHelper varus_h = new VarusHelper();
                    SilpoHelper silpo_h = new SilpoHelper();
                    MetroHelper metro_h = new MetroHelper();
                    if (CompareProductsItems != null)
                    {
                        var atb_product = atb_h.GetProductInformation(CompareProductsItems.PAtb != null ? CompareProductsItems.PAtb.OriginalLink : "");
                        var varus_product = varus_h.GetProductInformation(CompareProductsItems.PVarus != null ? CompareProductsItems.PVarus.OriginalLink : "");
                        var silpo_product = silpo_h.GetProductInformation(CompareProductsItems.PSilpo != null ? CompareProductsItems.PSilpo.OriginalLink : "");
                        var metro_product = metro_h.GetProductInformation(CompareProductsItems.PMetro != null ? CompareProductsItems.PMetro.OriginalLink : "");
                        await Task.WhenAll(atb_product, varus_product, silpo_product, metro_product);

                        var atb_ = atb_product.Result;
                        var varus_ = varus_product.Result;
                        var silpo_ = silpo_product.Result;
                        var metro_ = metro_product.Result;

                        if (UserInfo != null)
                        {
                            var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                            if (User != null)
                            {
                                var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                                if (atb_ != null)
                                {
                                    var InFavorites_a = MyFavorites.Where(x => x.Name.Trim().ToLower() == atb_.Name.Trim().ToLower()).FirstOrDefault();
                                    if (InFavorites_a != null)
                                    {
                                        atb_.IsFavorites = true;
                                    }
                                    else
                                    {
                                        atb_.IsFavorites = false;
                                    }
                                }
                                if (varus_ != null)
                                {
                                    var InFavorites_v = MyFavorites.Where(x => x.Name.Trim().ToLower() == varus_.Name.Trim().ToLower()).FirstOrDefault();
                                    if (InFavorites_v != null)
                                    {
                                        varus_.IsFavorites = true;
                                    }
                                    else
                                    {
                                        varus_.IsFavorites = false;
                                    }
                                }
                                if (silpo_ != null)
                                {
                                    var InFavorites_s = MyFavorites.Where(x => x.Name.Trim().ToLower() == silpo_.Name.Trim().ToLower()).FirstOrDefault();
                                    if (InFavorites_s != null)
                                    {
                                        silpo_.IsFavorites = true;
                                    }
                                    else
                                    {
                                        silpo_.IsFavorites = false;
                                    }
                                }
                                if (metro_ != null)
                                {
                                    var InFavorites_m = MyFavorites.Where(x => x.Name.Trim().ToLower() == metro_.Name.Trim().ToLower()).FirstOrDefault();
                                    if (InFavorites_m != null)
                                    {
                                        metro_.IsFavorites = true;
                                    }
                                    else
                                    {
                                        metro_.IsFavorites = false;
                                    }
                                }
                            }
                        }
                        response.atb_products = atb_;
                        response.varus_products = varus_;
                        response.silpo_products = silpo_;
                        response.metro_products = metro_;
                        return Json(response);
                    }
                    else
                    {
                        return Ok("No data");
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> SetCompareProducts(CompareProducts cp_)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            try
            {
                session["CompareProductsItems"] = cp_;
                session["CompareProducts"] = true;
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> IsAuth()
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                if (UserInfo != null)
                {
                    var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                    if (User != null)
                    {
                        var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                        UserInfo.MyFavorites = MyFavorites;
                        mdb.Dispose();
                    }
                    return Json(UserInfo);
                }
                else
                {
                    return BadRequest("Не авторизований користувач");
                }

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> DeleteFromMyFavorites(ProductItem pi)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                if (UserInfo == null)
                {
                    mdb.Dispose();
                    return BadRequest("Потрібна авторизація");
                }
                else
                {
                    var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                    if (User == null)
                    {
                        mdb.Dispose();
                        return BadRequest("Користувача не існує!");
                    }
                    else
                    {
                        var InMyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id && x.Name.Trim() == pi.Name.Trim()).FirstOrDefault();
                        if (InMyFavorites != null)
                        {
                            mdb.FavoriteProducts.Remove(InMyFavorites);
                            mdb.SaveChanges();
                            var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                            UserInfo.MyFavorites = MyFavorites;
                            mdb.Dispose();
                            return Json(UserInfo);
                        }
                        else
                        {
                            mdb.Dispose();
                            return BadRequest("Товар не знайдено в ваших вподобаннях!");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> AddToMyFavorites(ProductItem pi)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                if (UserInfo == null)
                {
                    mdb.Dispose();
                    return BadRequest("Потрібна авторизація");
                }
                else
                {
                    var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                    if (User == null)
                    {
                        mdb.Dispose();
                        return BadRequest("Користувача не існує!");
                    }
                    else
                    {
                        var InMyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id && x.Name.Trim() == pi.Name.Trim()).FirstOrDefault();
                        if (InMyFavorites != null)
                        {
                            mdb.Dispose();
                            return BadRequest("Товар вже знаходиться в обраному!");
                        }
                        else
                        {
                            FavoriteProducts fp = new FavoriteProducts();
                            fp.Name = pi.Name;
                            fp.Owner = pi.Owner;
                            fp.OriginalLink = pi.OriginalLink;
                            fp.UserID = User.Id;
                            mdb.FavoriteProducts.Add(fp);
                            mdb.SaveChanges();
                            var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                            UserInfo.MyFavorites = MyFavorites;
                            mdb.Dispose();
                            return Json(UserInfo);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        public async Task<IHttpActionResult> GetMyFavorites()
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                if (UserInfo == null)
                {
                    mdb.Dispose();
                    return BadRequest("Потрібна авторизація");
                }
                else
                {
                    var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                    if (User == null)
                    {
                        mdb.Dispose();
                        return BadRequest("Користувача не існує!");
                    }
                    else
                    {
                        List<Task<ProductItem>> tasks_ = new List<Task<ProductItem>>();
                        var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                        mdb.Dispose();
                        foreach (var item in MyFavorites)
                        {
                            if (item.Owner.Trim() == "varus")
                            {
                                VarusHelper varus_h = new VarusHelper();
                                var varus_product = varus_h.GetProductInformation(item.OriginalLink);
                                tasks_.Add(varus_product);
                            }
                            else if (item.Owner.Trim() == "atb")
                            {
                                AtbHelper atb_h = new AtbHelper();
                                var atb_product = atb_h.GetProductInformation(item.OriginalLink);
                                tasks_.Add(atb_product);
                            }
                            else if (item.Owner.Trim() == "silpo")
                            {
                                SilpoHelper silpo_h = new SilpoHelper();
                                var silpo_product = silpo_h.GetProductInformation(item.OriginalLink);
                                tasks_.Add(silpo_product);
                            }
                            else if (item.Owner.Trim() == "metro")
                            {
                                MetroHelper metro_h = new MetroHelper();
                                var metro_product = metro_h.GetProductInformation(item.OriginalLink);
                                tasks_.Add(metro_product);
                            }
                        }
                        if (tasks_.Count < 1)
                        {
                            return BadRequest("No data");
                        }
                        else
                        {
                            await Task.WhenAll(tasks_);
                            var answer_ = new List<ProductItem>();
                            foreach (var item in tasks_)
                            {
                                answer_.Add(item.Result);
                            }
                            foreach (var item in answer_)
                            {
                                item.IsFavorites = true;
                            }
                            return Json(answer_);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> LogOut()
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            try
            {
                if (UserInfo != null)
                {
                    session["AuthUser"] = null;
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Authorization(AuthData ad)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                var user_ = mdb.Users.Where(x => x.Login.Trim().ToLower() == ad.Login.Trim().ToLower() && x.Password.Trim().ToLower() == ad.Password.Trim().ToLower()).FirstOrDefault();
                if (user_ != null)
                {
                    AuthUser UserInfo = new AuthUser();
                    UserInfo.Login = user_.Login;
                    UserInfo.IsAuth = true;
                    if (session != null)
                    {
                        session["AuthUser"] = UserInfo;
                    }
                    mdb.Dispose();
                    return Json(UserInfo);
                }
                else
                {
                    mdb.Dispose();
                    return BadRequest("Користувача не існує!");
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Registration(AuthData ad)
        {
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                var user_ = mdb.Users.Where(x => x.Login.Trim().ToLower() == ad.Login.Trim().ToLower()).FirstOrDefault();
                if (user_ == null)
                {
                    Users us_ = new Users();
                    us_.Login = ad.Login.Trim().ToLower();
                    us_.Password = ad.Password.Trim().ToLower();
                    mdb.Users.Add(us_);
                    mdb.SaveChanges();
                    mdb.Dispose();
                    return Ok();
                }
                else
                {
                    mdb.Dispose();
                    return BadRequest("Користувач з таким логіном вже існує!");
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }
        }
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Subscribe(MailData md)
        {
            MyShopDBEntities mdb = new MyShopDBEntities();
            try
            {
                var sub = mdb.Subscriptions.Where(x => x.Email.Trim().ToLower() == md.Email.Trim().ToLower()).FirstOrDefault();
                if (sub == null)
                {
                    Subscriptions s = new Subscriptions();
                    s.Email = md.Email.Trim().ToLower();
                    mdb.Subscriptions.Add(s);
                    mdb.SaveChanges();
                    mdb.Dispose();
                    return Ok();
                }
                else
                {
                    mdb.Dispose();
                    return BadRequest("Ви вже підписані на розсилку!");
                }
            }
            catch (Exception e)
            {
                mdb.Dispose();
                return BadRequest(e.Message);
            }

        }
        [HttpGet]
        public async Task<IHttpActionResult> SalesGet()
        {

            AtbHelper atb_h = new AtbHelper();

            var s_a = atb_h.SaleProducts();
            VarusHelper varus_h = new VarusHelper();
            var s_v = varus_h.SaleProducts();
            SilpoHelper silpo_h = new SilpoHelper();
            var s_s = silpo_h.SaleProducts();
            MetroHelper metro_h = new MetroHelper();
            var s_m = metro_h.SaleProducts();
            await Task.WhenAll(s_a, s_v, s_s, s_m);
            dynamic response = new ExpandoObject();
            response.ATB_Sale = s_a.Result;
            response.Varus_Sale = s_v.Result;
            response.Silpo_Sale = s_s.Result;
            response.Metro_Sale = s_m.Result;
            return Json(response);
        }
        [HttpGet]
        public async Task<IHttpActionResult> AllRecommendedGet()
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
  
            var vegetables_all = new List<ProductItem>();

            var meats_all = new List<ProductItem>();

            var fish_all = new List<ProductItem>();

            AtbHelper atb_h = new AtbHelper();

            var r_a_vegetables = atb_h.RecommendedProducts("289-ovochi");

            var r_a_meats = atb_h.RecommendedProducts("343-m-yaso-ta-yaytsya");

            var r_a_fish = atb_h.RecommendedProducts("353-riba-i-moreprodukti");
            VarusHelper varus_h = new VarusHelper();
            var r_v_vegetables = varus_h.RecommendedProducts("frukti-ovochi-gorihi/ovochi-svizhi");
            var r_v_meats = varus_h.RecommendedProducts("myasni-virobi-ta-yaycya/myaso-svizhe");
            var r_v_fish = varus_h.RecommendedProducts("riba-ta-moreprodukti/riba");
            SilpoHelper silpo_h = new SilpoHelper();
            var r_s_vegetables = silpo_h.RecommendedProducts("ovochi-378");
            var r_s_meats = silpo_h.RecommendedProducts("svizhe-m-iaso-278");
            var r_s_fish = silpo_h.RecommendedProducts("zhyva-ta-okholodzhena-ryba-ta-moreprodukty-280");
            MetroHelper metro_h = new MetroHelper();
            var r_m_vegetables = metro_h.RecommendedProducts("vegetables-metro/");
            var r_m_meats = metro_h.RecommendedProducts("fresh-meat-metro/");
            var r_m_fish = metro_h.RecommendedProducts("fresh-fish-metro/");
            await Task.WhenAll(r_a_vegetables, r_a_meats, r_a_fish, r_v_vegetables, r_v_meats, r_v_fish, r_s_vegetables, r_s_meats, r_s_fish, r_m_vegetables, r_m_meats, r_m_fish);

            vegetables_all.AddRange(r_a_vegetables.Result);
            vegetables_all.AddRange(r_v_vegetables.Result);
            vegetables_all.AddRange(r_s_vegetables.Result);
            vegetables_all.AddRange(r_m_vegetables.Result);

            meats_all.AddRange(r_a_meats.Result);
            meats_all.AddRange(r_v_meats.Result);
            meats_all.AddRange(r_s_meats.Result);
            meats_all.AddRange(r_m_meats.Result);

            fish_all.AddRange(r_a_fish.Result);
            fish_all.AddRange(r_v_fish.Result);
            fish_all.AddRange(r_s_fish.Result);
            fish_all.AddRange(r_m_fish.Result);

            if (UserInfo != null)
            {
                var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                if (User != null)
                {
                    var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                    foreach (var item in vegetables_all)
                    {
                        var InFavorites = MyFavorites.Where(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()).FirstOrDefault();
                        if (InFavorites != null)
                        {
                            item.IsFavorites = true;
                        }
                        else
                        {
                            item.IsFavorites = false;
                        }
                    }
                    foreach (var item in meats_all)
                    {
                        var InFavorites = MyFavorites.Where(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()).FirstOrDefault();
                        if (InFavorites != null)
                        {
                            item.IsFavorites = true;
                        }
                        else
                        {
                            item.IsFavorites = false;
                        }
                    }
                    foreach (var item in fish_all)
                    {
                        var InFavorites = MyFavorites.Where(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()).FirstOrDefault();
                        if (InFavorites != null)
                        {
                            item.IsFavorites = true;
                        }
                        else
                        {
                            item.IsFavorites = false;
                        }
                    }
                }
            }
            vegetables_all.Shuffle();
            meats_all.Shuffle();
            fish_all.Shuffle();
            dynamic response = new ExpandoObject();
            response.vegetables_all = vegetables_all;
            response.meats_all = meats_all;
            response.fish_all = fish_all;
            return Json(response);
        }
        [HttpGet()]
        public async Task<IHttpActionResult> ProductsFromCategoryGet(string category)
        {
            var session = HttpContext.Current != null ? HttpContext.Current.Session : null;
            var UserInfo = session != null ? session["AuthUser"] as AuthUser : null;
            MyShopDBEntities mdb = new MyShopDBEntities();
            var all_products = new List<ProductItem>();
            try
            {
                var CurrentCategory_ = Extension.comparison.Where(x => x.Key == category).FirstOrDefault();
                AtbHelper atb_h = new AtbHelper();
                VarusHelper varus_h = new VarusHelper();
                SilpoHelper silpo_h = new SilpoHelper();
                MetroHelper metro_h = new MetroHelper();
                var atb_products = atb_h.AllProductsInTheCategory(CurrentCategory_.Value.AtbCategoryName);
                var varus_products = varus_h.AllProductsInTheCategory(CurrentCategory_.Value.VarusCategoryName);
                var silpo_products = silpo_h.AllProductsInTheCategory(CurrentCategory_.Value.SilpoCategoryName);
                var metro_products = metro_h.AllProductsInTheCategory(CurrentCategory_.Value.MetroCategoryName);
                await Task.WhenAll(atb_products, varus_products, silpo_products, metro_products);
                all_products.AddRange(atb_products.Result);
                all_products.AddRange(varus_products.Result);
                all_products.AddRange(silpo_products.Result);
                all_products.AddRange(metro_products.Result);
                if (UserInfo != null)
                {
                    var User = mdb.Users.Where(x => x.Login.Trim().ToLower() == UserInfo.Login.ToLower()).FirstOrDefault();
                    if (User != null)
                    {
                        var MyFavorites = mdb.FavoriteProducts.Where(x => x.UserID == User.Id).ToList();
                        foreach (var item in all_products)
                        {
                            var InFavorites = MyFavorites.Where(x => x.Name.Trim().ToLower() == item.Name.Trim().ToLower()).FirstOrDefault();
                            if (InFavorites != null)
                            {
                                item.IsFavorites = true;
                            }
                            else
                            {
                                item.IsFavorites = false;
                            }
                        }
                    }
                }
                all_products.Shuffle();
            }
            catch (Exception e) { }
            dynamic response = new ExpandoObject();
            response.all_products = all_products;
            return Json(response);
        }
        [HttpGet()]
        public async Task<IHttpActionResult> SearchProducts(string search)
        {
            dynamic response = new ExpandoObject();
            try
            {
                AtbHelper atb_h = new AtbHelper();
                VarusHelper varus_h = new VarusHelper();
                SilpoHelper silpo_h = new SilpoHelper();
                MetroHelper metro_h = new MetroHelper();
                var atb_products = atb_h.SearchProducts(search);
                var varus_products = varus_h.SearchProducts(search);
                var silpo_products = silpo_h.SearchProducts(search);
                var metro_products = metro_h.SearchProducts(search);
                await Task.WhenAll(atb_products, varus_products, silpo_products, metro_products);
                response.atb_products = atb_products.Result;
                response.varus_products = varus_products.Result;
                response.silpo_products = silpo_products.Result;
                response.metro_products = metro_products.Result;
            }
            catch (Exception e) { }
            return Json(response);
        }
    }
}