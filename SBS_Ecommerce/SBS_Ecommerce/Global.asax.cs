using log4net;
using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SBS_Ecommerce
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            log.Error("App_Error", ex);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            GetSiteIDFromHost();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            GetSiteIDFromHost();
        }

        private int GetSiteIDFromHost()
        {
            var host = HttpContext.Current.Request.Url.AbsoluteUri;
            string domain = string.Empty; ;
            int siteID = 0;
            string urlNonHttp = host.Substring(host.IndexOf("//") + 2);
            string[] lsSub = urlNonHttp.Split('/');
            if (lsSub != null && lsSub.Count() > 0)
            {

                int indexofSub = lsSub[0].IndexOf(".");
                if (indexofSub > 0)
                {
                    domain = lsSub[0].Substring(0, indexofSub);
                }
                else
                {
                    domain = lsSub[0];
                }
            }

            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, domain));
            var json = JsonConvert.DeserializeObject<CompanyDTO>(value);
            var company = json.Items;
            var context = new RequestContext(
                new HttpContextWrapper(System.Web.HttpContext.Current),
                new RouteData());
            var urlHelper = new UrlHelper(context);
            HttpContext ctx = HttpContext.Current;
            
            if (company != null)
            {
                siteID = company.Company_ID;
                SBS_Entities db = new SBS_Entities();
                var exist = db.Themes.Where(m => m.CompanyId == siteID);
                if (exist == null || exist.Count() <= 0)
                {
                    ctx.Response.Redirect(urlHelper.Content("~/InstallPage/Index?cpID="+siteID));
                }
            }
            else
            {
                ctx.Response.Redirect(urlHelper.Content("~/Error/PageNotFound"));
            }
            return siteID;
        }
    }
}
