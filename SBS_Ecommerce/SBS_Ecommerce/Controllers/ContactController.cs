using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Utilities;
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
        private string Clientid = ConfigurationManager.AppSettings["CompanyID"].ToString();

        // GET: Contact
        public ActionResult Index()
        {
            var pathView = GetLayout() + ContactPath;
            var company = SBSCommon.Instance.GetCompany();
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