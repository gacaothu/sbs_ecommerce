using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using SBS_Ecommerce.Models.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class NotificationController : BaseController
    {
        // GET: Notification
        private SBS_Ecommerce.Models.SBS_Entities db = new SBS_Ecommerce.Models.SBS_Entities();
        #region Send mail
        public ActionResult CustomerNotificationEmail()
        {

            return View();
        }

        public ActionResult SendMail(string orderId)
        {
            SendMailNotification(orderId, 12);
            return View();
        }
        public void SendMailNotification(string orderId, int idCustomer)
        {
            var customer = db.Users.Find(idCustomer);
            var emailAccount = db.EmailAccounts.FirstOrDefault();

            //Order
            var order = db.Orders.Find(orderId);
            //Order model email
            var emailModel = new EmailNotificationDTO();

            var mailUtil = new EmailUtil(emailAccount.Email, emailAccount.DisplayName,
                emailAccount.Password, emailAccount.Host, emailAccount.Port);
            var nameCustomer = customer.FirstName + " " + customer.LastName;

            var lstOrderDetail = db.OrderDetails.Where(o => o.OrderId == orderId).ToList();
            var lstOrderDetailModel = AutoMapper.Mapper.Map<List<OrderDetail>, List<OrderDetailDTO>>(lstOrderDetail);

            emailModel.ListOrderEmail = lstOrderDetailModel;
            emailModel.User = customer;
            emailModel.Order = order;
            emailModel.OrderStatus = this.GetOrderStatus(order);

            var bodyEmail = RenderPartialViewToString("CustomerNotificationEmail", emailModel);
            var subjectEmail = "Order " + emailModel.OrderStatus + " " + orderId;
            mailUtil.SendEmail(customer.Email, nameCustomer, subjectEmail, bodyEmail, true);
        }
        /// <summary>
        /// Render partial view to string
        /// </summary>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model</param>
        /// <returns>Result</returns>
        public string RenderPartialViewToString(string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }
        private string GetOrderStatus(Order order)
        {
            if (order.OrderStatus == (int)OrderStatus.Cancelled)
            {
                return "Cancelled";
            }
            if (order.OrderStatus == (int)OrderStatus.Completed)
            {
                return "Complete";
            }
            if (order.OrderStatus == (int)OrderStatus.Pending)
            {
                return "Pending";
            }
            if (order.OrderStatus == (int)OrderStatus.Processing)
            {
                return "Processing";
            }
            return null;
        }

        #endregion
    }
}