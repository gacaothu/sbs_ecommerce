using SBS_Ecommerce.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class BlogController : BaseController
    {
        private const string PathTheme = "~/Views/Theme/";
        private const string BlogPath = "/Blog/Index.cshtml";
        Helper helper = new Helper();
        // GET: Blog
        public ActionResult Index()
        {
            var pathView = GetLayout() + BlogPath;
            return View(pathView);
        }

        public ActionResult Detail()
        {
            return View();
        }

    }
}