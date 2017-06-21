using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class HomeController : BaseController
    {
        private const string PathTheme = "~/Views/Theme/";
        private const string IndexPath = "/Index.cshtml";
        private const string ConfigMenu = "/configmenu.xml";
        private const string ConfigLayout = "/layout.xml";
        private const string ConfigTheme = "~/Content/theme.xml";

        private SBSUnitWork unitWork;

        public HomeController()
        {
            unitWork = new SBSUnitWork();
        }

        public ActionResult Index()
        {
            var themes = GetThemeActive();
            var pathView = themes.PathView + IndexPath;

            List<ConfigMenu> lstMenu = unitWork.Repository<ConfigMenu>().GetAll(m=>m.CompanyId == cId).OrderBy(m => m.Position).ToList();
            ViewBag.RenderMenu = lstMenu;

            ViewBag.RenderLayout = unitWork.Repository<ConfigLayout>().GetAll(m => m.CompanyId == cId && m.Active).OrderBy(m => m.Position).ToList();

            try
            {
                ViewBag.LstBlog = unitWork.Repository<Blog>().GetAll(m => m.CompanyId == cId).ToList();
            }
            catch
            {
            }

            var configChat = GetConfigChatting();
            if (configChat != null)
            {
                ViewBag.PageID = configChat.PageID;
            }

            CategoryDTO resultCategory = new CategoryDTO();
            string valueCategory = RequestUtil.SendRequest(SBSConstants.GetListCategory);
            try
            {
                resultCategory = JsonConvert.DeserializeObject<CategoryDTO>(valueCategory);
                ViewBag.LstCategory = resultCategory.Items;
            }
            catch
            {
            }

            // SEO
            //InitSEO(Request.Url.Scheme, Request.Url.Host, Request.FilePath);
            return View(pathView);
        }    

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult SearchProduct(string text)
        {
            var product = SBSCommon.Instance.GetSearchProducts(text);
            return Json(product);
        }
    }
}