using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBS_Ecommerce.Framework.Configurations
{
    public class CustomAuthorize: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            string controller = filterContext.RouteData.Values["controller"].ToString();
            filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = controller, action = "Login" }));
        }
    }
}