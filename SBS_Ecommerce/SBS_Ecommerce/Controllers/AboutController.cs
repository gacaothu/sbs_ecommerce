using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class AboutController : BaseController
    {
        private const string AboutPath = "/About/Index.cshtml";
        // GET: About
        public ActionResult Index()
        {
            var pathView = GetLayout() + AboutPath;
            return View(pathView);
        }
    }
}