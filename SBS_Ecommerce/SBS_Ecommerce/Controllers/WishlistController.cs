using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
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
        SBS_DevEntities db = new SBS_DevEntities();

        // GET: Wishlist
        public ActionResult GetWishlist()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            LoggingUtil.StartLog(ClassName, methodName);

            var result = db.Wishlists.ToList();

            var viewPath = GetLayout() + WishlistPath;

            LoggingUtil.EndLog(ClassName, methodName);
            return View(viewPath);
        }

        /// <summary>
        /// Inserts to Wishlist.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsertToWishlist(int productId)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            LoggingUtil.StartLog(ClassName, methodName);

            int userId = GetIdUserCurrent();
            try
            {
                db.Wishlists.Add(new Wishlist { UId = userId, ProId = productId, Status = SBSConstants.Active });
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