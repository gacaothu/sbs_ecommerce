using SBSECommerge.Framework.Utilities;
using SBSECommerge.Models;
using System.Web.Mvc;

namespace SBSECommerge.Controllers
{
    public class HomeController : Controller
    {
        private const string ClassName = nameof(HomeController);
        private SBS_DevEntities db = EntityUtil.GetEntity();

        public ActionResult Index()
        {
            LoggingUtil.StartLog(ClassName, System.Reflection.MethodBase.GetCurrentMethod().Name);
            
            LoggingUtil.EndLog(ClassName, System.Reflection.MethodBase.GetCurrentMethod().Name);
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