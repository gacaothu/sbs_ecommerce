using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
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
        private const string Domain = "http://qa.bluecube.com.sg/pos3v2-wserv/";
        private const string SaveReview = "WServ/SaveProductReview";
        private const string DeletedReview = "WServ/DeleteProductReview";
        private const string PathMiniCart = "/Product/_PartialMiniCart.cshtml";

        private const int PriceAsc = 1;
        private const int PriceDesc = 2;
        private const int NameAsc = 3;
        private const int NameDesc = 4;


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

            ViewBag.Data = result.Items;

            //Get product review
            List<ProductReview> lstProducReview = SBSCommon.Instance.GetLstProductReview(id);

            //Send data list product review
            ViewBag.LstReview = lstProducReview;

            //Caculate rating
            var rate = Convert.ToInt32(lstProducReview.Average(m => m.Rate));

            ViewBag.Rate = rate;
            ViewBag.Currency = SBSCommon.Instance.GetCompany().Currency_Code;

            int userId = GetIdUserCurrent();
            bool addedWishlist = false;
            if (userId == SBSConstants.Failed)
            {
                addedWishlist = false;
            }
            else
            {
                var data = db.GetWishlists.Where(m => m.UId == userId && m.ProId == id).FirstOrDefault();
                if (data != null)
                {
                    addedWishlist = true;
                }
            }
            ViewBag.AddedWishlist = addedWishlist;
            return View(pathView, db.GetUsers.Where(m => m.Id == userId).FirstOrDefault());
        }

        /// <summary>
        /// Checkouts this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Checkout()
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            var pathView = theme.Path + PathCheckout;
            ViewBag.Company = SBSCommon.Instance.GetCompany();

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
            Models.Base.Cart cart = new Models.Base.Cart();

            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                cart.LstOrder = new List<Models.Base.Order>();
            }
            cart.Discount = 0;
            cart.Counpon = string.Empty;
            foreach (var item in cart.LstOrder)
            {
                item.Product.IsApplyCoupon = false;
            }

            List<Product> products = SBSCommon.Instance.GetProducts();
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();

            bool successAdd = false;
            foreach (var item in cart.LstOrder)
            {
                item.Product.IsApplyCoupon = false;
                if (item.Product.Product_ID == id)
                {
                    item.Count = item.Count + count;
                    cart.Total = cart.Total + count * (item.Product.Promotion_ID != -1 ? double.Parse(item.Product.Promotion_Price.ToString()) : item.Product.Selling_Price);
                    successAdd = true;
                    break;
                }
            }

            if (!successAdd)
            {
                if (product.Stocked_Quantity <= 0 && !product.Allowable_PreOrder)
                {
                    return Json(new { Partial = "" }, JsonRequestBehavior.AllowGet);
                }
                Models.Base.Order orderItem = new Models.Base.Order();
                orderItem.Product = product;
                orderItem.Count = count;
                cart.Total = cart.Total + count * (orderItem.Product.Promotion_ID != -1 ? double.Parse(orderItem.Product.Promotion_Price.ToString()) : orderItem.Product.Selling_Price);
                cart.LstOrder.Add(orderItem);
            }

            double tax = SBSCommon.Instance.GetTaxOfProduct();
            if (tax > 0)
            {
                tax = SBSExtensions.ConvertMoneyDouble(cart.Total * tax / 100);
            }
            cart.Tax = tax;
            cart.Total = SBSExtensions.ConvertMoneyDouble(cart.Total);
            Session["Cart"] = cart;

            //If exist login save to cart of user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                var cartOfDatabase = db.Carts.Where(m => m.UserId == userID && m.ProID == id).FirstOrDefault();
                if (cartOfDatabase != null)
                {
                    cartOfDatabase.Quantity = cartOfDatabase.Quantity + count;
                    db.SaveChanges();
                }
                else
                {
                    cartOfDatabase = new Models.Cart();
                    cartOfDatabase.CompanyId = cId;
                    cartOfDatabase.ProID = id;
                    cartOfDatabase.Quantity = count;
                    cartOfDatabase.UserId = userID;
                    cartOfDatabase.IsPreOrder = product.Allowable_PreOrder;
                    cartOfDatabase.PreOrderNotice = product.Delivery_Noted;
                    db.Carts.Add(cartOfDatabase);
                    db.SaveChanges();

                }

            }

            string miniCartView = PartialViewToString(this, GetLayout() + PathMiniCart, null);
            return Json(new { Partial = miniCartView }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveItemCart(int id)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            cart.Discount = 0;
            cart.Counpon = string.Empty;
            foreach (var item in cart.LstOrder)
            {
                item.Product.IsApplyCoupon = false;
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
                        cart.Total = cart.Total - (item.Product.Promotion_ID != -1 ? double.Parse(item.Product.Promotion_Price.ToString()) : item.Product.Selling_Price);
                        double tax = SBSCommon.Instance.GetTaxOfProduct();
                        if (tax > 0)
                        {
                            tax = cart.Total * tax / 100;
                        }
                        cart.Tax = tax;
                    }
                }
            }

            //If exist login save to cart of user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                var cartOfDatabase = db.Carts.Where(m => m.UserId == userID && m.ProID == id).FirstOrDefault();
                if (cartOfDatabase != null && cartOfDatabase.Quantity > 1)
                {
                    cartOfDatabase.Quantity = cartOfDatabase.Quantity - 1;
                    db.SaveChanges();
                }
            }
            Session["Cart"] = cart;

            string miniCartView = PartialViewToString(this, GetLayout() + PathMiniCart, null);
            return Json(new { Partial = miniCartView }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ApplyCouponCode(string couponCode)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();

            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            cart.Counpon = string.Empty;

            string pIds = "&pIds=";
            foreach (var item in cart.LstOrder)
            {
                pIds = pIds + item.Product.Product_ID + "&pIds=";
            }
            pIds = pIds.Substring(0, pIds.Length - 6);
            List<PromotionCoupon> promotionCouponOfProduct = SBSCommon.Instance.GetPromotionCouponOfProduct(couponCode, pIds);
            cart.Discount = -2;
            foreach (var item in cart.LstOrder)
            {
                item.Product.IsApplyCoupon = false;
            }

            if (promotionCouponOfProduct.Count == 0)
            {
                Session["Cart"] = cart;
                return Json(new { discount = -1 }, JsonRequestBehavior.AllowGet);
            }
            //set total price equal zero and save coupon code to cart
            double discount = 0;
            cart.Counpon = couponCode;
            foreach (var item in promotionCouponOfProduct)
            {
                var product = cart.LstOrder.Where(p => p.Product.Product_ID == item.Product_ID && item.Priority > p.Product.Priority).FirstOrDefault();
                if (product != null)
                {
                    product.Product.IsApplyCoupon = true;
                    discount = discount + ((product.Product.Selling_Price - item.Promotion_Price) * product.Count);
                }
            }

            //Caculator again total price 
            double totalPrice = 0;
            foreach (var item in cart.LstOrder)
            {
                if (item.Product.IsApplyCoupon || item.Product.Promotion_ID == -1)
                {
                    totalPrice = totalPrice + item.Count * item.Product.Selling_Price;
                }
                else
                {
                    totalPrice = totalPrice + (double)item.Count * item.Product.Promotion_Price.Value;
                }

            }
            cart.Total = SBSExtensions.ConvertMoneyDouble(totalPrice);
            cart.Discount = discount;
            //If exist login save to cart of user

            Session["Cart"] = cart;

            return Json(new { discount = discount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Removes the cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult RemoveCart(int id)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

            //Reset coupon
            cart.Discount = 0;
            cart.Counpon = string.Empty;
            foreach (var item in cart.LstOrder)
            {
                item.Product.IsApplyCoupon = false;
            }
            List<Product> products = SBSCommon.Instance.GetProducts();

            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();
            for (int i = 0; i < cart.LstOrder.Count; i++)
            {
                if (cart.LstOrder[i].Product.Product_ID == product.Product_ID)
                {
                    cart.Total = cart.Total - ((cart.LstOrder[i].Product.Promotion_ID != -1 ? double.Parse(cart.LstOrder[i].Product.Promotion_Price.ToString()) : cart.LstOrder[i].Product.Selling_Price) * cart.LstOrder[i].Count);
                    cart.LstOrder.RemoveAt(i);
                    double tax = SBSCommon.Instance.GetTaxOfProduct();
                    if (tax > 0)
                    {
                        tax = cart.Total * tax / 100;
                    }
                    cart.Tax = tax;
                    break;
                }
            }

            //If exist login save to cart of user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                var cartOfDatabase = db.Carts.Where(m => m.UserId == userID && m.ProID == id).FirstOrDefault();
                if (cartOfDatabase != null)
                {
                    db.Carts.Remove(cartOfDatabase);
                    db.SaveChanges();
                }
            }

            Session["Cart"] = cart;
            string miniCartView = PartialViewToString(this, GetLayout() + PathMiniCart, null);
            return Json(new { Partial = miniCartView }, JsonRequestBehavior.AllowGet);
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

            int pNo = 1;
            int pLength = 100;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProductByCategory, cId, pNo, pLength, id));
            ProductListDTO result = new ProductListDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);
                ViewBag.Data = result.Items;
                ViewBag.Categories = SBSCommon.Instance.GetCategories();
                Session[SBSConstants.SessionCategoryKey + cId] = result.Items;
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
        public ActionResult Search(string keyWord,string sort,string sortType,int? cgID,string lstBrandID,string lstRangeID,bool filter,int currentPage)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var pathView = GetLayout() + PathSearch;
            ProductListDTO result = new ProductListDTO();
            try
            {
                string brandQry = null;
                string rangeQry = null;

                int pNo = 1;
                int pLength = 1000;
                StringBuilder searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithoutCategory, cId, pNo, pLength, keyWord, sort, sortType));
                if (cgID != null)
                {
                    searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithCategory, cId, pNo, pLength, keyWord, sort, sortType, cgID));
                }
                if (!lstBrandID.IsNullOrEmpty())
                {
                    string[] lstBrand = lstBrandID.Split('_');
                    foreach (var item in lstBrand)
                    {
                        brandQry += "&brandID=" + item;
                    }
                }
                if (!lstRangeID.IsNullOrEmpty())
                {
                    string[] lstRange = lstRangeID.Split('_');
                    foreach (var item in lstRange)
                    {
                        rangeQry += "&rangeID=" + item;
                    }
                }

                searchBuilder.Append(brandQry);
                searchBuilder.Append(rangeQry);
                string value = RequestUtil.SendRequest(searchBuilder.ToString());

                result = JsonConvert.DeserializeObject<ProductListDTO>(value);

                ViewBag.Data = result.Items.Skip((currentPage - 1) * SBSConstants.MaxItem).Take(SBSConstants.MaxItem).ToList();
                ViewBag.DataCount = result.Items.Count;
                ViewBag.Keyword = keyWord;
                ViewBag.Categories = SBSCommon.Instance.GetCategories();
                ViewBag.Brands = SBSCommon.Instance.GetBrands();
                ViewBag.PriceRange = SBSCommon.Instance.GetPriceRange();

                if(!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(sortType))
                {
                    ViewBag.Sort = sort;
                    ViewBag.sortType = sortType;
                }
                Session[SBSConstants.SessionSearchProductKey + cId] = result.Items;
                Session[SBSConstants.SessionSearchKey + cId] = keyWord;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                ViewBag.Data = new List<Product>();
            }

            if (filter)
            {
                ViewBag.DataCount = result.Items.Count;
                ViewBag.CurrentPage = currentPage;
                LoggingUtil.EndLog(ClassName, methodName);
                return Json(new { Partial = PartialViewToString(this, GetLayout() + PathPartialSearch, ViewBag.Data), Count = result.Items.Count, Keyword = keyWord },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                LoggingUtil.EndLog(ClassName, methodName);
                return View(pathView);
            }
        }

        ///// <summary>
        ///// Searches the specified term.
        ///// </summary>
        ///// <param name="term">The term.</param>
        ///// <returns></returns>
        //public ActionResult Search(SearchViewModel model, string keyWord, string sort, string sortType, int? cgID, string lstBrandID, string lstRangeID, bool filter, int? currentPage)
        //{
        //    string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
        //    LoggingUtil.StartLog(ClassName, methodName);

        //    var pathView = GetLayout() + PathSearch;
        //    ProductListDTO result = new ProductListDTO();
        //    try
        //    {
        //        string brandQry = null;
        //        string rangeQry = null;

        //        int pNo = 1;
        //        int pLength = 1000;
        //        StringBuilder searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithoutCategory, cId, pNo, pLength, model.Keyword, model.Sort, model.SortType));
        //        if (model.CgID != null)
        //        {
        //            searchBuilder = new StringBuilder(string.Format(SBSConstants.SearchProductWithCategory, cId, pNo, pLength, model.Keyword, model.Sort, model.SortType, model.CgID));
        //        }
        //        if (!model.BrandID.IsNullOrEmpty())
        //        {
        //            foreach (var item in model.BrandID)
        //            {
        //                brandQry += "&brandID=" + item;
        //            }
        //        }
        //        if (!model.RangeID.IsNullOrEmpty())
        //        {
        //            foreach (var item in model.RangeID)
        //            {
        //                rangeQry += "&rangeID=" + item;
        //            }
        //        }

        //        searchBuilder.Append(brandQry);
        //        searchBuilder.Append(rangeQry);
        //        string value = RequestUtil.SendRequest(searchBuilder.ToString());

        //        result = JsonConvert.DeserializeObject<ProductListDTO>(value);

        //        ViewBag.Data = result.Items.Skip((model.CurrentPage - 1) * SBSConstants.MaxItem).Take(SBSConstants.MaxItem).ToList();
        //        ViewBag.DataCount = result.Items.Count;
        //        ViewBag.Keyword = model.Keyword;
        //        ViewBag.Categories = SBSCommon.Instance.GetCategories();
        //        ViewBag.Brands = SBSCommon.Instance.GetBrands();
        //        ViewBag.PriceRange = SBSCommon.Instance.GetPriceRange();
        //        Session[SBSConstants.SessionSearchProductKey + cId] = result.Items;
        //        Session[SBSConstants.SessionSearchKey + cId] = model.Keyword;
        //    }
        //    catch (Exception e)
        //    {
        //        LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
        //        ViewBag.Data = new List<Product>();
        //    }

        //    if (model.Filter)
        //    {
        //        ViewBag.DataCount = result.Items.Count;
        //        ViewBag.CurrentPage = model.CurrentPage;
        //        LoggingUtil.EndLog(ClassName, methodName);
        //        return Json(new { Partial = PartialViewToString(this, GetLayout() + PathPartialSearch, ViewBag.Data), Count = result.Items.Count, Keyword = model.Keyword },
        //            JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        LoggingUtil.EndLog(ClassName, methodName);
        //        return View(pathView);
        //    }
        //}

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
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["Product_ID"] = prID.ToString();
                values["Title"] = title;
                values["Rate"] = rate.ToString();
                values["Comment"] = comment;
                values["Name"] = name;
                var userID = GetIdUserCurrent();
                if (userID != -1)
                {
                    values["Commentator_ID"] = userID.ToString();
                }
                var response = client.UploadValues(Domain + SaveReview, values);
                var responseString = Encoding.Default.GetString(response);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Deletes the review.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult DeleteReview(int id)
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["pRwID"] = id.ToString();
                var response = client.UploadValues(Domain + DeletedReview, values);
                var responseString = Encoding.Default.GetString(response);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the review.
        /// </summary>
        /// <param name="reviewID">The review identifier.</param>
        /// <param name="productID">The product identifier.</param>
        /// <returns></returns>
        public ActionResult GetReview(int reviewID, int productID)
        {
            var userID = GetIdUserCurrent();
            var productReview = SBSCommon.Instance.GetLstProductReview(productID).Where(m => m.Commentator_ID == userID && m.Product_Review_ID == reviewID).FirstOrDefault();
            return Json(new { Title = productReview.Title, Content = productReview.Comment, Name = productReview.Name, Rate = productReview.Rate }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edits the review.
        /// </summary>
        /// <param name="rate">The rate.</param>
        /// <param name="title">The title.</param>
        /// <param name="name">The name.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult EditReview(int rate, string title, string name, string comment, int id, int productID)
        {
            var userID = GetIdUserCurrent();
            var productReview = SBSCommon.Instance.GetLstProductReview(productID).Where(m => m.Commentator_ID == userID && m.Product_Review_ID == id).FirstOrDefault();
            if (productReview != null)
            {
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["Product_Review_ID"] = id.ToString();
                    values["Product_ID"] = productID.ToString();
                    values["Title"] = title;
                    values["Rate"] = rate.ToString();
                    values["Comment"] = comment;
                    values["Name"] = name;
                    values["Commentator_ID"] = userID.ToString();
                    var response = client.UploadValues(Domain + SaveReview, values);
                    var responseString = Encoding.Default.GetString(response);
                }
            }

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

            string viewStr = "";
            try
            {
                ViewBag.Data = SortProduct(orderby, (List<Product>)Session[SBSConstants.SessionCategoryKey + cId], currentPage);
                ViewBag.DataCount = ((List<Product>)Session[SBSConstants.SessionCategoryKey + cId]).Count;
                ViewBag.CurrentPage = currentPage;
                viewStr = PartialViewToString(this, GetLayout() + PathPartialCategory, ViewBag.Data);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

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
        public ActionResult NavigatePage(int orderby, int currentPage = 1)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            List<Product> tmpProducts = (List<Product>)Session[SBSConstants.SessionSearchProductKey + cId];
            ViewBag.Data = SortProduct(orderby, tmpProducts, currentPage);
            ViewBag.DataCount = tmpProducts.Count;
            ViewBag.CurrentPage = currentPage;
            LoggingUtil.EndLog(ClassName, methodName);
            return PartialView(GetLayout() + PathPartialSearch, ViewBag.Data);
        }

        private List<Product> SortProduct(int orderby, List<Product> tmpProducts, int currentPage)
        {
            if (!tmpProducts.IsNullOrEmpty())
            {
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
                if (currentPage >= 1)
                {
                    tmpProducts = tmpProducts.Skip((currentPage - 1) * SBSConstants.MaxItem).Take(SBSConstants.MaxItem).ToList();
                }
            }

            return tmpProducts;
        }
    }
}