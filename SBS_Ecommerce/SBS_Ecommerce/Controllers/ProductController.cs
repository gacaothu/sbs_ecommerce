using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ProductController : BaseController
    {
        private const string ClassName = nameof(HomeController);
        private const string PathDetail = "/Product/Detail.cshtml";
        private const string PathCheckout = "/Product/Checkout.cshtml";
        private const string PathCategory = "/Product/Category.cshtml";
        private const string PathSearch = "/Product/Search.cshtml";
        private const string PathPartialSearch = "/Product/_PartialSearch.cshtml";
        private const string PathPartialCategory = "/Product/_PartialCategory.cshtml";

        private const int PriceAsc = 1;
        private const int PriceDesc = 2;
        private const int NameAsc = 3;
        private const int NameDesc = 4;

        private SBS_Entities db = new SBS_Entities();

        /// <summary>
        /// Detailses the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var layout = GetLayout();
            var pathView = layout + PathDetail;

            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetProduct, id));
            ProductDetailDTO result = new ProductDetailDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductDetailDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            //Send data list product review
            ViewBag.Review = db.ProductReviews.Where(m => m.ProId == id).ToList();

            //Send data user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                ViewBag.User = db.Users.Where(m => m.Id == userID).FirstOrDefault();
            }

            return View(pathView, result.Items);
        }

        /// <summary>
        /// Checkouts this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            var pathView = GetLayout() + PathCheckout;
            return View(pathView);
        }

        /// <summary>
        /// Adds the cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
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
                cart.LstOrder = new List<Models.Base.Order>();
            }

            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            int cId = 1;
            int pNo = 1;
            int pLength = 50;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            ProductListDTO result = new ProductListDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            var products = result.Items;
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();

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
                Models.Base.Order orderItem = new Models.Base.Order();
                orderItem.Product = product;
                orderItem.Count = count;
                cart.Total = cart.Total + count * orderItem.Product.Selling_Price;
                cart.LstOrder.Add(orderItem);
            }

            Session["Cart"] = cart;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Removes the cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
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
            LoggingUtil.StartLog(ClassName, methodName);
            int cId = 1;
            int pNo = 1;
            int pLength = 50;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            ProductListDTO result = new ProductListDTO();

            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }
            var products = result.Items;
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();
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

        /// <summary>
        /// Categories the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Category(int id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var layout = GetLayout();
            var pathView = layout + PathCategory;

            int cId = 1;
            int pNo = 1;
            int pLength = 50;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProductByCategory, cId, pNo, pLength, id));
            ProductListDTO result = new ProductListDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
                ViewBag.Data = result.Items;
                SBSCommon.Instance.SetTempProductByCategory(result.Items);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            ViewBag.CategoryName = SBSCommon.Instance.GetCategories().Where(m => m.Category_ID == id).FirstOrDefault().Category_Name;
            return View(pathView);
        }

        /// <summary>
        /// Searches the specified term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns></returns>
        public ActionResult Search(string term)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var pathView = GetLayout() + PathSearch;

            try
            {
                ViewBag.Data = SearchProduct(term);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                ViewBag.Data = new List<Product>();
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return View(pathView);


        }

        /// <summary>
        /// Add review product
        /// </summary>
        /// <param name="rate">rating</param>
        /// <param name="title">title</param>
        /// <param name="name">name</param>
        /// <param name="comment">comment</param>
        /// <param name="prID">product id</param>
        /// <returns></returns>
        public ActionResult ReviewProduct(int rate, string title, string name, string comment, int prID)
        {
            ProductReview prReview = new ProductReview();
            var userID = GetIdUserCurrent();

            if (userID != -1)
            {
                prReview.UId = userID;
            }

            prReview.Content = comment;
            prReview.CreatedAt = DateTime.Now;
            prReview.ProId = prID;
            prReview.Rating = rate;
            prReview.Title = title;
            prReview.NameCreated = name;

            db.ProductReviews.Add(prReview);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReview(int id)
        {
            var userID = GetIdUserCurrent();
            var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            if (userID != productReview.UId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            db.ProductReviews.Remove(productReview);
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReview(int id)
        {
            var userID = GetIdUserCurrent();
            var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            if (userID != productReview.UId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Title = productReview.Title, Content = productReview.Content, Name = productReview.NameCreated, Rate = productReview.Rating }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditReview(int rate, string title, string name, string comment, int id)
        {
            var userID = GetIdUserCurrent();
            var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            if (userID != productReview.UId)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            productReview.Rating = rate;
            productReview.Title = title;
            productReview.NameCreated = name;
            productReview.Content = comment;

            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Sorts the product.
        /// </summary>
        /// <param name="fromPage">From page.</param>
        /// <param name="orderby">The orderby.</param>
        /// <param name="currentPage">The current page.</param>
        /// <returns></returns>
        public ActionResult SortProduct(string fromPage, int orderby, int currentPage = 1)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            if (fromPage.Contains("Category"))
            {
                var tmpProducts = SBSCommon.Instance.GetTempProductByCategory();
                ViewBag.Data = SortProduct(orderby, tmpProducts, currentPage);
                LoggingUtil.EndLog(ClassName, methodName);
                return PartialView(GetLayout() + PathPartialCategory, ViewBag.Data);
            }
            else
            {
                var tmpProducts = SBSCommon.Instance.GetTempSearchProducts();
                ViewBag.Data = SortProduct(orderby, tmpProducts, currentPage);
                LoggingUtil.EndLog(ClassName, methodName);
                return PartialView(GetLayout() + PathPartialSearch, ViewBag.Data);
            }
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTags()
        {
            var tags = SBSCommon.Instance.GetTags();
            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        private List<Product> SortProduct(int orderby, List<Product> tmpProducts, int currentPage)
        {
            if (!tmpProducts.IsNullOrEmpty())
            {
                if (currentPage >= 1)
                {
                    tmpProducts = tmpProducts.Skip((currentPage - 1) * SBSConstants.MaxItem).Take(SBSConstants.MaxItem).ToList();
                }

                switch (orderby)
                {
                    case PriceAsc:
                        tmpProducts = tmpProducts.OrderBy(m => m.Selling_Price).ToList();
                        break;
                    case PriceDesc:
                        tmpProducts = tmpProducts.OrderByDescending(m => m.Selling_Price).ToList();
                        break;
                    case NameAsc:
                        tmpProducts = tmpProducts.OrderBy(m => m.Product_Name).ToList();
                        break;
                    case NameDesc:
                        tmpProducts = tmpProducts.OrderByDescending(m => m.Product_Name).ToList();
                        break;
                    default:
                        break;
                }
            }

            return tmpProducts;
        }

        private List<Product> SearchProduct(string term)
        {
            int cId = 1;
            int pNo = 1;
            int pLength = 1000;

            string value = RequestUtil.SendRequest(string.Format(SBSConstants.SearchProduct, cId, pNo, pLength, term));
            ProductListDTO result = new ProductListDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
                SBSCommon.Instance.SetTempSearchProducts(result.Items);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);

            }
            return result.Items;
        }

    }
}