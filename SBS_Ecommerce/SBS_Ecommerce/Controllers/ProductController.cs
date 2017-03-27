using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ProductController : BaseController
    {
        private const string className = nameof(HomeController);
        private const string DetailsAction = "/Product/Details.cshtml";
        private const string PathCheckout = "/Product/Checkout.cshtml";

        // GET: Product
        public ActionResult Details(int id)
        {
            var layout = GetLayout();
            var pathView = layout + DetailsAction;

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetProduct, id));
            ProductDetailDTO result = new ProductDetailDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductDetailDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }
            return View(pathView, result);
        }

        public ActionResult Checkout()
        {
            var pathView = GetLayout() + PathCheckout;
            return View(pathView);
        }

        public ActionResult AddCart(int id, int count)
        {
            //Get session Cart
            Cart cart = new Cart();
            if (Session["Cart"] != null)
            {
                cart = (Cart)Session["Cart"];
            }
            else
            {
                cart.LstOrder = new List<Order>();
            }

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            int cId = 1;
            int pNo = 1;
            int pLength = 10;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            ProductListDTO result = new ProductListDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            var product = result.Items.Where(m => m.Product_ID == id).FirstOrDefault();

            bool successAdd = false;
            foreach (var item in cart.LstOrder)
            {
                if (item.Product.Product_ID == id)
                {
                    item.Count = item.Count + count;
                    cart.Total = cart.Total + count * item.Product.Selling_Price;
                    successAdd = true;
                    break;
                }
            }

            if (!successAdd)
            {
                Order orderItem = new Order();
                orderItem.Product = product;
                orderItem.Count = count;
                cart.Total = cart.Total + count * orderItem.Product.Selling_Price;
                cart.LstOrder.Add(orderItem);
            }

            Session["Cart"] = cart;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveCart(int id)
        {
            //Get session Cart
            Cart cart = new Cart();
            if (Session["Cart"] != null)
            {
                cart = (Cart)Session["Cart"];
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            int cId = 1;
            int pNo = 1;
            int pLength = 10;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            ProductListDTO result = new ProductListDTO();

            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            var product = result.Items.Where(m => m.Product_ID == id).FirstOrDefault();
            for (int i = 0; i < cart.LstOrder.Count; i++)
            {
                if (cart.LstOrder[i].Product.Product_ID == product.Product_ID)
                {
                    cart.Total = cart.Total - (cart.LstOrder[i].Product.Selling_Price * cart.LstOrder[i].Count);
                    cart.LstOrder.RemoveAt(i);
                    break;
                }
            }

            Session["Cart"] = cart;
            return Json(true, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Checkout");
        }

    }
}