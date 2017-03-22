using Newtonsoft.Json;
using SBSECommerce.Framework.Configurations;
using SBSECommerce.Framework.Utilities;
using SBSECommerce.Models;
using SBSECommerge.Models.DTOs;
using System;
using System.Web.Mvc;

namespace SBSECommerce.Controllers
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