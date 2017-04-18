using Microsoft.AspNet.Identity;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminOrderMgmtController : BaseController
    {
        private const string ClassName = nameof(AdminOrderMgmtController);
        private const string PathOrder = "~/Views/Admin/Orders.cshtml";
        private const string PathPartialOrder = "~/Views/Admin/_PartialOrder.cshtml";
        private const string PathPartialDetail = "~/Views/Admin/_PartialOrderDetail.cshtml";

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
                ViewBag.CompleteOrders = GetOrders(OrderStatus.Completed);
                ViewBag.CancelOrders = GetOrders(OrderStatus.Cancelled);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return View(Url.Content(PathOrder));
        }

        /// <summary>
        /// Get detail of Order.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult OrderDetail(string id)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            List<OrderDetail> details = new List<OrderDetail>();
            try
            {
                details = db.OrderDetails.Where(m => m.OrderId == id).ToList();
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }
            ViewBag.Data = details;
            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { Partial = PartialViewToString(this, Url.Content(PathPartialDetail), ViewBag.Data) }, JsonRequestBehavior.AllowGet);
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
                        flag = true;
                        order.ShippingStatus = (int)OrderStatus.Processing;
                        break;
                    case (int)OrderStatus.Processing:
                        flag = true;
                        order.ShippingStatus = (int)OrderStatus.Completed;
                        break;
                    default:
                        flag = false;
                        break;
                }

                if (flag)
                {
                    order.UpdatedAt = DateTime.Now;
                    var entry = db.Entry(order);
                    entry.Property(m => m.ShippingStatus).IsModified = true;
                    entry.Property(m => m.UpdatedAt).IsModified = true;
                    db.SaveChanges();
                    flag = true;
                }
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

        /// <summary>
        /// Refreshes the tab.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public ActionResult RefreshTab()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            string partialPending = "";
            string partialProcessing = "";
            string partialCompleted = "";
            try
            {
                // Get pending order content
                ViewBag.Data = GetOrders(OrderStatus.Pending);
                partialPending = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

                // Get processing order content
                ViewBag.Data = GetOrders(OrderStatus.Processing);
                partialProcessing = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

                // Get complete order content
                ViewBag.Data = GetOrders(OrderStatus.Completed);
                partialCompleted = PartialViewToString(this, PathPartialOrder, ViewBag.Data);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new
            {
                Pending = partialPending,
                Processing = partialProcessing,
                Completed = partialCompleted
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Filters the order.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="sortByDate">The sort by date.</param>
        /// <param name="dateFrom">From date.</param>
        /// <param name="dateTo">To date.</param>
        /// <returns></returns>
        public ActionResult FilterOrder(int? status, string sortByDate, string dateFrom, string dateTo)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            string filterPending = "";
            string filterProcessing = "";
            string filterCompleted = "";
            string filterCanceled = "";
            try
            {
                // filter pending order 
                ViewBag.Data = FilterOrder(OrderStatus.Pending, status, sortByDate, dateFrom, dateTo);
                filterPending = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

                // filter processing order 
                ViewBag.Data = FilterOrder(OrderStatus.Processing, status, sortByDate, dateFrom, dateTo);
                filterProcessing = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

                // filter completed order 
                ViewBag.Data = FilterOrder(OrderStatus.Completed, status, sortByDate, dateFrom, dateTo);
                filterCompleted = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

                // filter canceled order
                ViewBag.Data = FilterOrder(OrderStatus.Cancelled, status, sortByDate, dateFrom, dateTo);
                filterCanceled = PartialViewToString(this, PathPartialOrder, ViewBag.Data);

            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        private List<Order> FilterOrder(OrderStatus kind, int? status, string sort, string dateFrom, string dateTo, int offset = 0, int limit = 100)
        {
            string asc = "acs";
            string desc = "desc";
            string spec = "spec";
            List<Order> result = new List<Order>();
            if (kind == OrderStatus.Processing)
            {

            }
            else
            {
                if (sort == asc)
                {
                    result = db.Orders.Where(m => m.ShippingStatus == (int)kind).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else if (sort == desc)
                {
                    result = db.Orders.Where(m => m.ShippingStatus == (int)kind).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else
                {
                    result = db.Orders.Where(m => m.ShippingStatus == (int)kind && m.CreatedAt == Convert.ToDateTime(dateFrom) && m.CreatedAt == Convert.ToDateTime(dateTo))
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
            }
            return result;
        }

        private List<Order> GetOrders(OrderStatus kind, int offset = 0, int limit = 100)
        {
            List<Order> result = new List<Order>();
            try
            {
                if (kind == OrderStatus.Completed)
                {
                    result = db.Orders.Where(m => m.ShippingStatus == (int)kind).OrderByDescending(m => m.UpdatedAt).Skip(offset).Take(limit).ToList();
                }
                else
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