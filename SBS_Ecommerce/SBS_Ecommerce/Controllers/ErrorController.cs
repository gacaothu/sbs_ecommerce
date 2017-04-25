using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult PageNotFound()
        {
            var company = Framework.SBSCommon.Instance.GetCompany();
            if (company == null)
                return View();
            else
                return Redirect(Url.Action("Index", "Home"));
        }
    }
}