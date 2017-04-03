using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ContactController : BaseController
    {
        private const string ContactPath = "/Contact/Index.cshtml";
        // GET: Contact
        public ActionResult Index()
        {
            var pathView = GetLayout() + ContactPath;
            return View(pathView);
        }
    }
}