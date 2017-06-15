using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Utilities;
using System.Net.Mail;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class ContactController : BaseController
    {
        private const string ContactPath = "/Contact/Index.cshtml";

        // GET: Contact
        public ActionResult Index()
        {
            //var theme = db.Themes.Where(m => m.CompanyId == cId && m.Active).FirstOrDefault();
            var theme = GetThemeActive();
            var pathView = theme.PathView + ContactPath;
            var company = SBSCommon.Instance.GetCompany();
            ViewBag.ThemeName = theme.Name;

            // SEO information
            InitSEO(Request.Url.Scheme, Request.Url.Host, Request.FilePath);
            return View(pathView, company);
        }

        public ActionResult SendMail(string name, string email, string message)
        {
            var company = SBSCommon.Instance.GetCompany();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(company.Email);
            mailMessage.Subject = "Send contact from email " + email;
            mailMessage.Body = message + "<br/><span>Email: " + email + "</span>";
            mailMessage.IsBodyHtml = true;

            EmailUtil emailUT = new EmailUtil();
            emailUT.SendListEmail(mailMessage);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}