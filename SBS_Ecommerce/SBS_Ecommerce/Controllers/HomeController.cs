﻿using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
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
        
        private const string className = nameof(HomeController);

        // List<Models.Base.Theme> themes = new List<Models.Base.Theme>();
        Helper helper = new Helper();
        public ActionResult Index()
        {
            //int pNo = 1;
            //int pLength = 10;
            //string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            //ProductListDTO result = new ProductListDTO();
            //try
            //{
            //    result = JsonConvert.DeserializeObject<ProductListDTO>(value);
            //}
            //catch (Exception e)
            //{
            //}

            //var themes = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault();
            var themes = db.GetThemes.FirstOrDefault(m => m.Active);
            var pathView = themes.PathView + IndexPath;

            //List<Layout> lstLayout = new List<Models.Base.Layout>();
            //try
            //{
            //    lstLayout = helper.DeSerializeLayout(Server.MapPath(PathTheme) + "/" + cId.ToString() + "/" + themes.Name + ConfigLayout);
            //}
            //catch (Exception e)
            //{
            //}

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath(PathTheme) + "/" + cId.ToString() + "/" + themes.Name + ConfigMenu);
            //ViewBag.RenderMenu = lstMenu.ToList();
            List<ConfigMenu> lstMenu = db.GetConfigMenus.OrderBy(m => m.Position).ToList();
            ViewBag.RenderMenu = lstMenu;

            //ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();
            ViewBag.RenderLayout = db.GetConfigLayouts.Where(m => m.Active).OrderBy(m => m.Position).ToList();

            try
            {
                ViewBag.LstBlog = db.GetBlogs.ToList();
            }
            catch (Exception e)
            {
            }


            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            CategoryDTO resultCategory = new CategoryDTO();
            string valueCategory = RequestUtil.SendRequest(SBSConstants.GetListCategory);

            try
            {
                resultCategory = JsonConvert.DeserializeObject<CategoryDTO>(valueCategory);
                ViewBag.LstCategory = resultCategory.Items;
            }
            catch (Exception e)
            {
            }

            // SEO
            InitSEO(Request.Url.Scheme, Request.Url.Host, Request.FilePath);
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