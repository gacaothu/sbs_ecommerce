using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ContactController : BaseController
    {
        private const string ContactPath = "/Contact/Index.cshtml";
        int cpID = SBSCommon.Instance.GetCompany().Company_ID;

        // GET: Contact
        public ActionResult Index()
        {
            var theme = db.Themes.Where(m => m.CompanyId == cpID && m.Active).FirstOrDefault();
            var pathView = theme.Path + ContactPath;
            var company = SBSCommon.Instance.GetCompany();
            ViewBag.ThemeName = theme.Name;
            return View(pathView, company);
        }

        public ActionResult SendMail(string name, string email, string message)
        {
            var company = SBSCommon.Instance.GetCompany();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(company.Email);
            mailMessage.Subject = "Send contact from email " + email;
            mailMessage.Body = message + "<br/><span>Email: "+email+"</span>";
            mailMessage.IsBodyHtml = true;

            EmailUtil emailUT = new EmailUtil();
            emailUT.SendListEmail(mailMessage);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}