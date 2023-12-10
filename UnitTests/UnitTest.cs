using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyShop.Controllers;
using MyShop.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace UnitTests
{
    [TestClass]
    public class UnitTestFreeController
    {
        [TestMethod]
        public async Task TestSearchProducts()
        {
            string search_str = "капуста";
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.SearchProducts(search_str);
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestProductsFromCategory()
        {
            string category_str = "vegetables";
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.ProductsFromCategoryGet(category_str);
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestAllRecommendedGet()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.AllRecommendedGet();
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestSalesGet()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.SalesGet();
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestGetCompareProducts()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.GetCompareProducts();
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestIsAuth()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            var result = await controller.IsAuth();
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            String deserialized = response.Result.Content.ReadAsStringAsync().Result;
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task TestRegistration()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            AuthData ad = new AuthData();
            ad.Login = "TestUser2";
            ad.Password = "TestPassword2";
            var result = await controller.Registration(ad);
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }

        [TestMethod]
        public async Task TestAuthorization()
        {
            var controller = new FreeController();
            controller.Configuration = new System.Web.Http.HttpConfiguration();
            controller.Request = new System.Net.Http.HttpRequestMessage();
            AuthData ad = new AuthData();
            ad.Login = "TestUser";
            ad.Password = "TestPassword";
            var result = await controller.Authorization(ad);
            var response = result.ExecuteAsync(new System.Threading.CancellationToken());
            Assert.IsNotNull(result);
            Assert.IsNotNull(response);
            Assert.AreEqual(HttpStatusCode.OK, response.Result.StatusCode);
        }
    }
}
