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
        private const string PathPartialPending = "~/Views/Admin/_PartialPendingOrders.cshtml";
        private const string PathPartialProcessing = "~/Views/Admin/_PartialProcessingOrders.cshtml";
        private const string PathPartialCompleted = "~/Views/Admin/_PartialCompletedOrders.cshtml";
        private const string PathPartialCanceled = "~/Views/Admin/_PartialCanceledOrders.cshtml";

        //private const string CountQuery = "Select count(OrderId) from [dbo].[Order] where OrderStatus = {0}";
        SBS_Ecommerce.Models.Extension.SBS_Entities dbRead = new Models.Extension.SBS_Entities();
        private SBS_Ecommerce.Models.SBS_Entities dbWrite = new SBS_Ecommerce.Models.SBS_Entities();

        /// <summary>
        /// Get Orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders(int kind)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            try
            {
                //ViewBag.Count = db.Database.SqlQuery<int>(string.Format(CountQuery, kind)).Single();

                switch (kind)
                {
                    case (int)OrderStatus.Pending:
                        ViewBag.Data = GetOrders(OrderStatus.Pending);
                        ViewBag.Partial = PartialViewToString(this, PathPartialPending, ViewBag.Data);                        
                        break;
                    case (int)OrderStatus.Processing:
                        ViewBag.Data = GetOrders(OrderStatus.Processing);
                        ViewBag.Partial = PartialViewToString(this, PathPartialProcessing, ViewBag.Data);
                        break;
                    case (int)OrderStatus.Completed:
                        ViewBag.Data = GetOrders(OrderStatus.Completed);
                        ViewBag.Partial = PartialViewToString(this, PathPartialCompleted, ViewBag.Data);
                        break;
                    case (int)OrderStatus.Cancelled:
                        ViewBag.Data = GetOrders(OrderStatus.Completed);
                        ViewBag.Partial = PartialViewToString(this, PathPartialCanceled, ViewBag.Data);
                        break;
                    default:
                        break;
                }
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
                details = dbRead.OrderDetails.Where(m => m.OrderId == id).ToList();
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
                var order = dbRead.Orders.FirstOrDefault(c => c.OrderId == id);

                switch (order.OrderStatus)
                {
                    case (int)OrderStatus.Pending:
                        flag = true;
                        order.OrderStatus = (int)OrderStatus.Processing;
                        order.ShippingStatus = (int)ShippingStatus.NotYetShipped;
                        break;
                    case (int)OrderStatus.Processing:
                        flag = true;
                        order.OrderStatus = (int)OrderStatus.Completed;
                        order.ShippingStatus = (int)ShippingStatus.Delivered;
                        break;
                    default:
                        flag = false;
                        break;
                }

                if (flag)
                {
                    order.UpdatedAt = DateTime.Now;
                    var entry = dbRead.Entry(order);
                    entry.Property(m => m.OrderStatus).IsModified = true;
                    entry.Property(m => m.ShippingStatus).IsModified = true;
                    entry.Property(m => m.UpdatedAt).IsModified = true;
                    dbWrite.SaveChanges();
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
                order = dbRead.Orders.FirstOrDefault(m => m.OrderId == id && m.ShippingStatus == shipingStatus);
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { Order = order }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Filters the order.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="sortByDate">The sort by date.</param>
        /// <param name="dateFrom">From date.</param>
        /// <param name="dateTo">To date.</param>
        /// <returns></returns>
        public ActionResult FilterOrder(int kind, int? status, string sortByDate, string dateFrom, string dateTo)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            string partialString = "";
            try
            {
                ViewBag.Data = ProcessFilter(kind, status, sortByDate, dateFrom, dateTo);
                partialString = PartialViewToString(this, PathPartialOrder, ViewBag.Data);
                ViewBag.Count = ViewBag.Data.Count;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return Json(new { Partial = partialString }, JsonRequestBehavior.AllowGet);
        }

        private List<Order> ProcessFilter(int kind, int? status, string sort, string dateFrom, string dateTo, int offset = 0, int limit = 100)
        {
            string asc = "asc";
            string desc = "desc";
            List<Order> result = new List<Order>();
            DateTime datefrom;
            DateTime dateto;
            if (kind == (int)OrderStatus.Processing)
            {
                if (sort == asc)
                {
                    result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else if (sort == desc)
                {
                    result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt >= datefrom)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        dateto = Convert.ToDateTime(dateTo);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt <= dateto)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        dateto = Convert.ToDateTime(dateTo);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt >= datefrom && m.CreatedAt <= dateto)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                }
            }
            else
            {
                if (sort == asc)
                {
                    result = dbRead.Orders.Where(m => m.OrderStatus == kind).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else if (sort == desc)
                {
                    result = dbRead.Orders.Where(m => m.OrderStatus == kind).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else
                {
                    
                    if (!string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.CreatedAt >= datefrom)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        dateto = Convert.ToDateTime(dateTo);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.CreatedAt <= dateto)
                            .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        dateto = Convert.ToDateTime(dateTo);
                        result = dbRead.Orders.Where(m => m.OrderStatus == kind && m.CreatedAt >= datefrom && m.CreatedAt <= dateto)
                            .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                }
            }
            return result;
        }

        private List<Order> GetOrders(OrderStatus kind)
        {
            List<Order> result = new List<Order>();
            try
            {
                result = dbRead.Orders.Where(m => m.OrderStatus == (int)kind).OrderBy(m => m.CreatedAt).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }
    }
}