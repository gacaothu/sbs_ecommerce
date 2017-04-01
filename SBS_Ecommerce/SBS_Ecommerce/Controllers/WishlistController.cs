using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using SBS_Ecommerce.Models.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    [Authorize]
    public class WishlistController : BaseController
    {
        private const string ClassName = nameof(WishlistController);
        private const string WishlistPath = "/Wishlist/Wishlist.cshtml";
        SBS_DEVEntities db = new SBS_DEVEntities();

        /// <summary>
        /// Gets the Wishlist.
        /// </summary>
        /// <returns></returns>
        public ActionResult Wishlist()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            var wishList = db.Wishlists.ToList();

            List<ProductDetailDTO> result = new List<ProductDetailDTO>();
            string value = "";
            foreach(var item in wishList)
            {
                value = RequestUtil.SendRequest(string.Format(SBSConstants.GetProduct, item.ProId));
                var product = JsonConvert.DeserializeObject<ProductDetailDTO>(value);
                result.Add(product);
            }
            var viewPath = GetLayout() + WishlistPath;

            LoggingUtil.EndLog(ClassName, methodName);
            return View(viewPath, result);
        }

        /// <summary>
        /// Inserts to Wishlist.
        /// </summary>
        /// <param name="id">The product identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertToWishlist(int id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            int userId = GetIdUserCurrent();
            try
            {
                var item = db.Wishlists.Where(m => m.ProId == id).FirstOrDefault();
                if (item != null)
                {
                    LoggingUtil.EndLog(ClassName, methodName);
                    return Json(new { reponse = SBSConstants.Exists });
                }
                db.Wishlists.Add(new Wishlist { UId = userId, ProId = id, Status = SBSConstants.Active });
                db.SaveChanges();
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                return Json(new { response = SBSConstants.Failed });
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { reponse = SBSConstants.Success });
        }

        /// <summary>
        /// Removes from Wishlist.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromWishlist(int id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            try
            {
                var item = db.Wishlists.Where(m => m.ProId == id).FirstOrDefault();
                db.Wishlists.Remove(item);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                return Json(new { response = SBSConstants.Failed });
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { reponse = SBSConstants.Success });
        }
    }
}