using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class AdminOrderMgmtController : BaseController
    {
        private const string ClassName = nameof(AdminOrderMgmtController);

        SBS_Entities db = new SBS_Entities();

        // GET: AdminOrderMgmt
        public ActionResult Orders()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            try
            {
                ViewBag.PendingOrders = GetOrders(OrderStatus.Pending);
                ViewBag.ProcessingOrders = GetOrders(OrderStatus.Processing);
                ViewBag.CompleteOrders = GetOrders(OrderStatus.Complete);
                ViewBag.CancelOrders = GetOrders(OrderStatus.Cancelled);
            }
            catch(Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }            

            LoggingUtil.EndLog(ClassName, methodName);
            return View(Url.Content("~/Views/Admin/Orders.cshtml"));
        }

        private List<Order> GetOrders(OrderStatus kind, int offset = 0, int limit = 100)
        {
            List<Order> result = new List<Order>();
            try
            {
                result = db.Orders.Where(m => m.ShippingStatus == (int)kind).OrderBy(m=>m.CreatedAt).Skip(offset).Take(limit).ToList();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }        
    }
}