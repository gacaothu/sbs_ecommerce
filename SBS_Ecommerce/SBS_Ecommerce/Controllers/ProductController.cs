using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        int cpID = SBSCommon.Instance.GetCompany().Company_ID;

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
            ViewBag.Review = new List<object>();

            //Send data user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                ViewBag.User = db.GetUsers.Where(m => m.Id == userID).FirstOrDefault();
            }

            //Caculate rating
            //var lstRate = db.ProductReviews.Where(m => m.ProId == id);
            //int totalRate = 0;
            //int rate = 0;
            //if (lstRate != null && lstRate.Count()>0)
            //{
            //    foreach (var item in lstRate)
            //    {
            //        if (item.Rating != null)
            //            totalRate = totalRate + (int)item.Rating;
            //    }
            //    rate = totalRate / lstRate.Count();
            //}
            //ViewBag.Rate = rate;
            return View(pathView, result.Items);
        }

        /// <summary>
        /// Checkouts this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            var pathView = theme.Path + PathCheckout;
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
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

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

            List<Product> products = SBSCommon.Instance.GetProducts();
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

        public ActionResult RemoveItemCart(int id)
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

            List<Product> products = SBSCommon.Instance.GetProducts();
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();
            foreach (var item in cart.LstOrder)
            {
                if (item.Product.Product_ID == id)
                {
                    if (item.Count > 0)
                    {
                        item.Count = item.Count - 1;
                        cart.Total = cart.Total - item.Product.Selling_Price;
                    }
                }
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
        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var pathView = GetLayout() + PathSearch;
            ProductListDTO result = new ProductListDTO();
            try
            {
                string brandQry = null;
                string rangeQry = null;

                int cId = 1;
                int pNo = 1;
                int pLength = 1000;
                StringBuilder searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithoutCategory, cId, pNo, pLength, model.Keyword, model.Sort, model.SortType));
                if (model.CgID != null)
                {
                    searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithCategory, cId, pNo, pLength, model.Keyword, model.Sort, model.SortType, model.CgID));
                }
                if (!model.BrandID.IsNullOrEmpty())
                {
                    foreach (var item in model.BrandID)
                    {
                        brandQry += "&brandID=" + item;
                    }
                }
                if (!model.RangeID.IsNullOrEmpty())
                {
                    foreach (var item in model.RangeID)
                    {
                        rangeQry += "&rangeID=" + item;
                    }
                }

                searchBuilder.Append(brandQry);
                searchBuilder.Append(rangeQry);
                string value = RequestUtil.SendRequest(searchBuilder.ToString());

                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
                SBSCommon.Instance.SetTempSearchProducts(result.Items);

                ViewBag.Data = result.Items;
                ViewBag.Term = model.Keyword;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                ViewBag.Data = new List<Product>();
            }

            if (model.Filter)
            {
                LoggingUtil.EndLog(ClassName, methodName);
                return Json(new { Partial = PartialViewToString(this, GetLayout() + PathPartialSearch, ViewBag.Data), Count = result.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                LoggingUtil.EndLog(ClassName, methodName);
                return View(pathView);
                //return Json(true, JsonRequestBehavior.AllowGet);
            }
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
            //ProductReview prReview = new ProductReview();
            //var userID = GetIdUserCurrent();

            //if (userID != -1)
            //{
            //    prReview.UId = userID;
            //}

            //prReview.Content = comment;
            //prReview.CreatedAt = DateTime.Now;
            //prReview.ProId = prID;
            ////prReview.Rating = rate;
            //prReview.Title = title;
            //prReview.NameCreated = name;

            //db.ProductReviews.Add(prReview);
            //db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteReview(int id)
        {
            //var userID = GetIdUserCurrent();
            //var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            //if (userID != productReview.UId)
            //{
            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}

            //db.ProductReviews.Remove(productReview);
            //db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReview(int id)
        {
            //var userID = GetIdUserCurrent();
            //var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            //if (userID != productReview.UId)
            //{
            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}

            //return Json(new { Title = productReview.Title, Content = productReview.Content, Name = productReview.NameCreated, Rate = productReview.Rating }, JsonRequestBehavior.AllowGet);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditReview(int rate, string title, string name, string comment, int id)
        {
            //var userID = GetIdUserCurrent();
            //var productReview = db.ProductReviews.Where(m => m.Id == id).FirstOrDefault();

            //if (userID != productReview.UId)
            //{
            //    return Json(false, JsonRequestBehavior.AllowGet);
            //}

            //productReview.Rating = rate;
            //productReview.Title = title;
            //productReview.NameCreated = name;
            //productReview.Content = comment;

            //db.SaveChanges();
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
            var tmpProducts = SBSCommon.Instance.GetTempProductByCategory();
            ViewBag.Data = SortProduct(orderby, tmpProducts, currentPage);
            string viewStr = PartialViewToString(this, GetLayout() + PathPartialCategory, ViewBag.Data);
            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { Partial = viewStr }, JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// Navigates the page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <returns></returns>
        public ActionResult NavigatePage(int currentPage = 1)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            ViewBag.Data = SBSCommon.Instance.GetTempSearchProducts().Skip((currentPage - 1) * SBSConstants.MaxItem).Take(SBSConstants.MaxItem).ToList();
            LoggingUtil.EndLog(ClassName, methodName);
            return PartialView(GetLayout() + PathPartialSearch, ViewBag.Data);
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
    }
}