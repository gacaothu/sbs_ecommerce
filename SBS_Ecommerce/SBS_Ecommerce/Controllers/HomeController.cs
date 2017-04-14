using log4net;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
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
        private SBS_Entities db = new SBS_Entities();
        List<Models.Base.Theme> themes = new List<Models.Base.Theme>();
        Helper helper = new Helper();
        public ActionResult Index()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            int cId = 1;
            int pNo = 1;
            int pLength = 10;
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
            ProductListDTO result = new ProductListDTO();
            //CompanyDTO company = new CompanyDTO();
            //BrandDTO brand = new BrandDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductListDTO>(value);

                // lấy dữ liệu của Company từ API
                //value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, cId));
                //value = RequestUtil.SendRequest(string.Format(SBSConstants.GetBrand, cId));

                // Parse dữ liệu từ API sang Model C#
                //company = JsonConvert.DeserializeObject<CompanyDTO>(value);
                //ViewBag.Company = company.Items;
                //brand = JsonConvert.DeserializeObject<BrandDTO>(value);
                //ViewBag.Brand = brand.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            themes = helper.DeSerialize(Server.MapPath(ConfigTheme));
            var pathView = GetLayout() + IndexPath;

            List<Models.Base.Layout> lstLayout = new List<Models.Base.Layout>();
            try
            {
                lstLayout = helper.DeSerializeLayout(Server.MapPath(PathTheme) + themes.Where(m => m.Active).FirstOrDefault().Name + ConfigLayout);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            themes = helper.DeSerialize(Server.MapPath(ConfigTheme));
            List<Models.Base.Menu> lstMenu = new List<Models.Base.Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath(PathTheme) + themes.Where(m => m.Active).FirstOrDefault().Name + ConfigMenu);
            ViewBag.RenderMenu = lstMenu.ToList();

            //Session["RenderLayout"] = lstLayout;
            ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();

            try
            {
                ViewBag.LstBlog = db.Blogs.ToList();
            }
            catch(Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }
            

            if (db.ConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.ConfigChattings.FirstOrDefault().PageID;

            CategoryDTO resultCategory = new CategoryDTO();
            string valueCategory = RequestUtil.SendRequest(SBSConstants.GetListCategory);

            try
            {
                resultCategory = JsonConvert.DeserializeObject<CategoryDTO>(valueCategory);
                ViewBag.LstCategory = resultCategory.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            LoggingUtil.EndLog(className, methodName);
            return View(pathView);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}