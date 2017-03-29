﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SBS_Ecommerce.Models;

namespace SBS_Ecommerce.Controllers
{
    public class OrdersController : BaseController
    {
        private SBS_DevEntities db = new SBS_DevEntities();

        private const string PurchaseHistoryPath = "/Orders/PurchaseHistory.cshtml";
        private const string PurchaseProcessPath = "/Orders/PurchaseProcess.cshtml";
        private const string CheckoutAddressPath = "/Orders/CheckoutAddress.cshtml";
        private const string CheckoutShippingPath = "/Orders/CheckoutShiping.cshtml";
        private const string CheckoutPaymentPath = "/Orders/CheckoutPayment.cshtml";
        // GET: Orders
        public ActionResult Index()
        {
            int id = GetIdUserCurrent();
            var order = (from pa in db.Payments
                         join o in db.Orders on pa.PaymentId equals o.PaymentId
                         where pa.UId == id
                         select new
                         {
                             OderId = o.OderId,
                             PaymentId = o.PaymentId,
                             DeliveryStatus = o.DeliveryStatus,
                             TotalAmount = o.TotalAmount,
                             CreatedAt = o.CreatedAt,
                             UpdatedAt = o.UpdatedAt
                         }).AsEnumerable().Select(x => new Order
                         {
                             OderId = x.OderId,
                             PaymentId = x.PaymentId,
                             DeliveryStatus = x.DeliveryStatus,
                             TotalAmount = x.TotalAmount,
                             CreatedAt = x.CreatedAt,
                             UpdatedAt = x.UpdatedAt
                         });

            var layout = GetLayout();
            var pathView = layout.Substring(0, layout.LastIndexOf("/")) + "/Orders/Index.cshtml";
            return View(pathView, order);
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }
        [HttpGet]
        public ActionResult PurchaseHistory()
        {
            var pathView = GetLayout() + PurchaseHistoryPath;
            return View(pathView);
        }
        [HttpGet]
        public ActionResult PurchaseProcess()
        {
            var pathView = GetLayout() + PurchaseProcessPath;
            return View(pathView);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OderId,PaymentId,CouponId,DeliveryStatus,TotalAmount,CreatedAt,UpdatedAt")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OderId,PaymentId,CouponId,DeliveryStatus,TotalAmount,CreatedAt,UpdatedAt")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        #region Checkout
        // GET: Orders/CheckoutSummary

        public ActionResult CheckoutSummary()
        {
            if (string.IsNullOrEmpty(CurrentUser.Name))
            {
                return RedirectToAction("Login", "Account",new { returnUrl="/Orders/CheckoutAddress" });
            }
            else
            {
                return RedirectToAction("CheckoutAddress");
            }
        }
        public ActionResult CheckoutAddress()
        {
            var pathView = GetLayout() + CheckoutAddressPath;



            return View(pathView);
        }

        public ActionResult CheckoutShipping()
        {
            var pathView = GetLayout() + CheckoutShippingPath;
            return View(pathView);
        }

        public ActionResult CheckoutPayment()
        {
            var pathView = GetLayout() + CheckoutPaymentPath;
            return View(pathView);
        }

        private List<SelectListItem> GetListUserAddress(int? selected = -1)
        {
            var listUserAddress = db.UserAddresses.ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = null, Value = null });
            foreach (var dataMember in listUserAddress)
            {
                var address = "";
                address += string.IsNullOrEmpty(dataMember.Address) ? dataMember.Address : "";
                address += string.IsNullOrEmpty(dataMember.City)? ", " + dataMember.City : "";
                address += string.IsNullOrEmpty(dataMember.State) ? ", " + dataMember.State : "";
                address += dataMember.ZipCode!=null ? " " + dataMember.ZipCode : "";
                //address += string.IsNullOrEmpty(dataMember.Co) ? ", " + dataMember.State : "";

                //if (selected == item.Id)
                //{
                    
                //    items.Add(new SelectListItem { Text = item.Ten, Value = item.Id.ToString(), Selected = true });
                //}
                //else
                //{
                //    items.Add(new SelectListItem { Text = item.Ten, Value = item.Id.ToString(), Selected = false });
                //}
            }
            return items;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
