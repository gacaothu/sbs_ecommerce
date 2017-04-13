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

        /// <summary>
        /// Get Orders.
        /// </summary>
        /// <returns></returns>
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
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return View(Url.Content("~/Views/Admin/Orders.cshtml"));
        }

        /// <summary>
        /// Updates the shipping status.
        /// </summary>
        /// <param name="id">The order identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateStatus(string id)
        {
            bool flag = false;
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            try
            {
                var order = db.Orders.FirstOrDefault(c => c.OderId == id);

                switch (order.ShippingStatus)
                {
                    case (int)OrderStatus.Pending:
                        order.ShippingStatus = (int)OrderStatus.Processing;
                        break;
                    case (int)OrderStatus.Processing:
                        order.ShippingStatus = (int)OrderStatus.Complete;
                        break;
                    case (int)OrderStatus.Complete:
                        break;
                    default:
                        break;
                }

                var entry = db.Entry(order);
                entry.Property(m => m.ShippingStatus).IsModified = true;
                db.SaveChanges();
                flag = true;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                flag = false;
            }

            LoggingUtil.EndLog(ClassName, methodName);
            if (flag)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Searches the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="shipingStatus">The shiping status.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string id, int shipingStatus)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            Order order = new Order();
            try
            {
                order = db.Orders.FirstOrDefault(m => m.OderId == id && m.ShippingStatus == shipingStatus);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { Order = order }, JsonRequestBehavior.AllowGet);
        }

        private List<Order> GetOrders(OrderStatus kind, int offset = 0, int limit = 100)
        {
            List<Order> result = new List<Order>();
            try
            {
                result = db.Orders.Where(m => m.ShippingStatus == (int)kind).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }
    }
}