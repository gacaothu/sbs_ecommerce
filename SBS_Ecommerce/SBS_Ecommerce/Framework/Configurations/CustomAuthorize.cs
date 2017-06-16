using System.Web.Mvc;
using System.Web.Routing;

namespace SBS_Ecommerce.Framework.Configurations
{
    public class CustomAuthorize: AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //string controller = filterContext.RouteData.Values["controller"].ToString();
            filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Admin", action = "Login" }));
        }
    }

    public class MasterAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //string controller = filterContext.RouteData.Values["controller"].ToString();
            filterContext.Result = new RedirectToRouteResult(new
            RouteValueDictionary(new { controller = "Master", action = "Login" }));
        }
    }
}