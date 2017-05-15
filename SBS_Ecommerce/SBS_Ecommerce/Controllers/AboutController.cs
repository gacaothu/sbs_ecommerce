using SBS_Ecommerce.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class AboutController : BaseController
    {
        private const string ConfigAbout = "/About/Index.cshtml";

        // GET: About
        /// <summary>
        /// Action about us
        /// </summary>
        /// <returns>Views</returns>
        public ActionResult Index()
        {
            ViewBag.Message = "Your application description page.";
            var pathTheme = BaseUtil.Instance.GetPathTheme() + ConfigAbout;
            return View(pathTheme);
        }
    }
}