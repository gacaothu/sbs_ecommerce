using SBS_Ecommerce.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class WishlistController : BaseController
    {
        private const string ClassName = nameof(WishlistController);
        private const string WishlistPath = "Wishlist/Wishlist.cshtml";

        // GET: Wishlist
        public ActionResult Wishlist()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            LoggingUtil.StartLog(ClassName, methodName);

            var viewPath = GetLayout() + WishlistPath;

            LoggingUtil.EndLog(ClassName, methodName);
            return View(viewPath);
        }
    }
}