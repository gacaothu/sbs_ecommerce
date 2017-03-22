using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private const string className = nameof(HomeController);
        private SBS_DevEntities db = EntityUtil.GetEntity();

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

            LoggingUtil.EndLog(className, methodName);
            return View(result);
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