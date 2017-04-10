using SBS_Admin.Models;
using SBS_Admin.Models.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Admin.Controllers
{
    public class AdminController : Controller
    {
        List<Theme> themes = new List<Theme>();
        private const string pathConfigTheme = "~/Content/theme.xml";
        private const string pathBlock = "~/Content/block.xml";
        private const string pathPage = "~/Content/page.xml";
        private SBS_DEVEntities db = new SBS_DEVEntities();
        Helper helper = new Helper();

        // GET: Admin
        public ActionResult Index()
        {

            string path = Path.GetFullPath(Path.Combine(Server.MapPath("~"), @"..\SBS_Ecommerce/FolderName"));

            return RedirectToAction("LayoutManager");
        }

    }
}