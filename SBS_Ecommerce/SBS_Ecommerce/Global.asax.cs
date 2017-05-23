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
using System.Web.Security;

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

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }
        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            log.Error("App_Error", ex);
        }

        protected void Session_End(object sender, EventArgs e)
        {
            //FormsAuthentication.SignOut();
            //Session.Clear();
            //Session.Abandon();
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var context = new RequestContext(
                new HttpContextWrapper(HttpContext.Current),
                new RouteData());
            var urlHelper = new UrlHelper(context);
            HttpContext ctx = HttpContext.Current;
            if (context.HttpContext.Session != null )
            {
                if(context.HttpContext.Session["Error_Page"] == null)
                {
                    if (context.HttpContext.Session["Company"] == null)
                    {
                        string domain = string.Empty;
                        int siteID = 0;
                        var host = HttpContext.Current.Request.Url.AbsoluteUri;
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

                        if (company != null)
                        {
                            siteID = company.Company_ID;
                            SBS_Entities db = new SBS_Entities();
                            var exist = db.Themes.Where(m => m.CompanyId == siteID);
                            context.HttpContext.Session["Company"] = company;
                            context.HttpContext.Session["Error_Page"] = null;
                            if (exist == null || exist.Count() <= 0)
                            {
                                ctx.Response.Redirect(urlHelper.Content("~/InstallPage/Index?cpID=" + siteID));
                            }
                        }
                        else
                        {
                            context.HttpContext.Session["Error_Page"] = true;
                            context.HttpContext.Session["Company"] = null;
                            ctx.Response.Redirect(urlHelper.Content("~/Error/PageNotFound"));
                        }
                    }
                    else
                    {
                        context.HttpContext.Session["Error_Page"] = null;
                    }
                }
               

            }
           
        }

    }
}
