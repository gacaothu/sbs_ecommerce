using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBS_Ecommerce
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                  name: "ThemeManager",
                  url: "Admin/{action}/{id}",
                  defaults: new { controller = "Admin", action = "ThemeManager", id = UrlParameter.Optional }
             );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ListShippingAddress",
                url: "Account/ListShippingAddress",
                defaults: new { controller = "Account", action = "ListShippingAddress", id = UrlParameter.Optional }
            );
            routes.MapRoute(
               name: "ListBillingAddress",
               url: "Account/ListBillingAddress",
               defaults: new { controller = "Account", action = "ListBillingAddress", id = UrlParameter.Optional }
           );
            routes.MapRoute(
               name: "ChangeAvatar",
               url: "Account/ChangeAvatar",
               defaults: new { controller = "Account", action = "ChangeAvatar", id = UrlParameter.Optional }
           );
            routes.MapRoute(
            name: "CheckoutPayment",
            url: "Orders/CheckoutPayment",
            defaults: new { controller = "Orders", action = "CheckoutPayment", id = UrlParameter.Optional }
        );
            routes.MapRoute(
           name: "CheckoutShipping",
           url: "Orders/CheckoutShipping",
           defaults: new { controller = "Orders", action = "CheckoutShipping", id = UrlParameter.Optional }
       );
        }
    }
}
