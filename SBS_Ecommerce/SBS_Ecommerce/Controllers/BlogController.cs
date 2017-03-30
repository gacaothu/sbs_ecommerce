using SBS_Ecommerce.Models;
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
        private const string BlogDetailPath = "/Blog/Detail.cshtml";
        Helper helper = new Helper();
        private SBS_DevEntities db = new SBS_DevEntities();
        // GET: Blog
        public ActionResult Index()
        {
            var pathView = GetLayout() + BlogPath;
            var lstBlock = db.Blogs.ToList();
            return View(pathView,lstBlock);
        }

        public ActionResult Detail(int id)
        {
            var blog = db.Blogs.Where(m => m.BlogId == id).FirstOrDefault();
            var pathView = GetLayout() + BlogDetailPath;
            return View(pathView, blog);
        }

    }
}