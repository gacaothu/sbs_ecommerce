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
    public class HomeController : Controller
    {
        private const string className = nameof(HomeController);
        private SBS_DevEntities db = new SBS_DevEntities();
        List<Theme> themes = new List<Theme>();
        Helper helper = new Helper();

        public ActionResult Index()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(className, methodName);
            string value = RequestUtil.SendRequest(SBSConstants.GetListProduct);
            ProductDTO result = new ProductDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductDTO>(value);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            var layOut = themes.Where(m => m.Active == true).FirstOrDefault().Path;
            Session["Layout"] = layOut;

            List<Layout> lstLayout = new List<Layout>();
            try
            {
                lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(className, methodName, e.Message);
            }

            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
            ViewBag.RenderMenu = lstMenu.ToList();

            //Session["RenderLayout"] = lstLayout;
            ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();
            
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
            return View();
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