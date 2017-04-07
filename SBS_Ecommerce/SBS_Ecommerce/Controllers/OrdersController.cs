using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configuration;
using log4net;
using SBS_Ecommerce.Models.Extension;
using System.Data.Entity.Validation;

namespace SBS_Ecommerce.Controllers
{
    public class OrdersController : BaseController
    {
        private SBS_Entities db = new SBS_Entities();

        private const string PurchaseHistoryPath = "/Orders/PurchaseHistory.cshtml";
        private const string PurchaseProcessPath = "/Orders/PurchaseProcess.cshtml";
        private const string CheckoutAddressPath = "/Orders/CheckoutAddress.cshtml";
        private const string CheckoutShippingPath = "/Orders/CheckoutShiping.cshtml";
        private const string CheckoutPaymentPath = "/Orders/CheckoutPayment.cshtml";
        protected static readonly ILog _logger = LogManager.GetLogger(typeof(OrdersController));


        // GET: Orders
        //public ActionResult Index()
        //{
        //    //int id = GetIdUserCurrent();
        //    //var order = (from pa in db.Payments
        //    //             join o in db.Orders on pa.PaymentId equals o.PaymentId
        //    //             where pa.UId == id
        //    //             select new
        //    //             {
        //    //                 OderId = o.OderId,
        //    //                 PaymentId = o.PaymentId,
        //    //                 DeliveryStatus = o.DeliveryStatus,
        //    //                 TotalAmount = o.TotalAmount,
        //    //                 CreatedAt = o.CreatedAt,
        //    //                 UpdatedAt = o.UpdatedAt
        //    //             }).AsEnumerable().Select(x => new Order
        //    //             {
        //    //                 OderId = x.OderId,
        //    //                 PaymentId = x.PaymentId,
        //    //                 DeliveryStatus = x.DeliveryStatus,
        //    //                 TotalAmount = x.TotalAmount,
        //    //                 CreatedAt = x.CreatedAt,
        //    //                 UpdatedAt = x.UpdatedAt
        //    //             });

        //    //var layout = GetLayout();
        //    //var pathView = layout.Substring(0, layout.LastIndexOf("/")) + "/Orders/Index.cshtml";
        //    //return View(pathView, order);
        //}

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
            if (string.IsNullOrEmpty(CurrentUser.Identity.Name))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Orders/CheckoutAddress" });
            }
            else
            {
                return RedirectToAction("CheckoutAddress");
            }
        }
        public ActionResult CheckoutAddress()
        {
            var pathView = GetLayout() + CheckoutAddressPath;
            int id = GetIdUserCurrent();
            var userAddress = db.UserAddresses.Where(u => u.Uid == id).ToList();
            //ViewBag.GetListUserAddress = GetListUserAddress();

            return View(pathView, userAddress);
        }

        public ActionResult CheckoutShipping()
        {
            var pathView = GetLayout() + CheckoutShippingPath;
            return View(pathView);
        }
        [HttpGet]
        public ActionResult CheckoutPayment()
        {
            ViewBag.CreditCardType = GetListCreditType();
            ViewBag.ExpireMonth = GetListMonthsCreditCard();
            ViewBag.ExpireYear = GetListYearsCreditCard();
            var pathView = GetLayout() + CheckoutPaymentPath;
            return View(pathView);
        }
        [HttpPost]
        public ActionResult CheckoutPayment(PaymentModel paymentModel)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();

            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var orderId = this.InsertDataOrder(cart, paymentModel);

            if (paymentModel.PaymentMethod == (int)PaymentMethod.CreditCard)
            {
                PaymentCreditCard(cart, paymentModel, orderId);
            }

            var pathView = GetLayout() + CheckoutPaymentPath;
            return View(pathView);
        }
        /// <summary>
        /// Function insert data order
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private string InsertDataOrder(Models.Base.Cart cart, PaymentModel paymentModel)
        {
            try
            {
                var order = new Order();
                var lstOrderDetail = new List<OrderDetail>();
                var idOrder = SBSExtensions.GetIdOrderUnique();
                order.OderId = idOrder;
                order.PaymentId = paymentModel.PaymentMethod;
                order.TotalAmount = cart.Total;
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                order.UId = GetIdUserCurrent();
                order.DeliveryStatus = ((int)PaymentStatus.Pending).ToString();
                order.Currency = "USD";
                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var detail in cart.LstOrder)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderId = idOrder;
                    orderDetail.ProId = detail.Product.Product_ID;
                    orderDetail.Price = detail.Product.Selling_Price;
                    orderDetail.ProductName = detail.Product.Product_Name;
                    orderDetail.ProductImg = detail.Product.Small_Img;
                    orderDetail.Quantity = detail.Count;
                    orderDetail.OrderType= ((int)OrderType.Order).ToString();
                    orderDetail.DeliveryStatus = ((int)PaymentStatus.Pending).ToString();
                    lstOrderDetail.Add(orderDetail);
                }
                db.OrderDetails.AddRange(lstOrderDetail);
                db.SaveChanges();
                return idOrder;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    _logger.ErrorFormat("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        _logger.ErrorFormat("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        private bool PaymentCreditCard(Models.Base.Cart cart, PaymentModel paymentModel, string orderId)
        {
            var id = GetIdUserCurrent();
            var user = db.Users.Find(id);
            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            PayPal.Api.Item item = new PayPal.Api.Item();
            List<PayPal.Api.Item> itms = new List<PayPal.Api.Item>();
            foreach (var order in cart.LstOrder)
            {
                item.name = order.Product.Product_Name;
                item.currency = "USD";
                item.price = order.Product.Selling_Price.ToString();
                item.quantity = order.Count.ToString();
                itms.Add(item);
            }
            PayPal.Api.ItemList itemList = new PayPal.Api.ItemList();
            itemList.items = itms;

            //Address for the payment
            PayPal.Api.Address billingAddress = new PayPal.Api.Address();


            //Address for the payment
            var userAddress = db.UserAddresses.Where(a => (a.Uid == GetIdUserCurrent() && a.DefaultType == true)).FirstOrDefault();
            billingAddress.city = userAddress.City;
            if (userAddress.Country == "Singapore")
            {
                billingAddress.country_code = "SG";
            }
            if (userAddress.Country == "Thailand")
            {
                billingAddress.country_code = "TH";
            }
            billingAddress.line1 = userAddress.Address;
            billingAddress.postal_code = userAddress.ZipCode.ToString();
            billingAddress.state = userAddress.State;

            //Now Create an object of credit card and add above details to it
            PayPal.Api.CreditCard crdtCard = new PayPal.Api.CreditCard();
            crdtCard.billing_address = billingAddress;
            crdtCard.cvv2 = paymentModel.CardCode;
            crdtCard.expire_month = paymentModel.ExpireMonth;
            crdtCard.expire_year = paymentModel.ExpireYear;
            crdtCard.first_name = user.FirstName;
            crdtCard.last_name = user.LastName;
            crdtCard.number = paymentModel.CardNumber;
            crdtCard.type = paymentModel.CreditCardType;

            // Specify details of your payment amount.
            PayPal.Api.Details details = new PayPal.Api.Details();
            details.shipping = "0";
            details.subtotal = "0";
            details.tax = "0";

            // Specify your total payment amount and assign the details object
            PayPal.Api.Amount amnt = new PayPal.Api.Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = cart.Total.ToString();
            amnt.details = details;


            // Now make a trasaction object and assign the Amount object
            PayPal.Api.Transaction tran = new PayPal.Api.Transaction();
            tran.amount = amnt;
            tran.description = "Payment amount form page SBS Ecommecer.";
            tran.item_list = itemList;
            tran.invoice_number = orderId;

            // Now, we have to make a list of trasaction and add the trasactions object
            // to this list. You can create one or more object as per your requirements

            List<PayPal.Api.Transaction> transactions = new List<PayPal.Api.Transaction>();
            transactions.Add(tran);

            // Now we need to specify the FundingInstrument of the Payer
            // for credit card payments, set the CreditCard which we made above

            PayPal.Api.FundingInstrument fundInstrument = new PayPal.Api.FundingInstrument();
            fundInstrument.credit_card = crdtCard;

            // The Payment creation API requires a list of FundingIntrument

            List<PayPal.Api.FundingInstrument> fundingInstrumentList = new List<PayPal.Api.FundingInstrument>();
            fundingInstrumentList.Add(fundInstrument);

            // Now create Payer object and assign the fundinginstrument list to the object
            PayPal.Api.Payer payr = new PayPal.Api.Payer();
            payr.funding_instruments = fundingInstrumentList;
            payr.payment_method = "credit_card";

            // finally create the payment object and assign the payer object & transaction list to it
            PayPal.Api.Payment pymnt = new PayPal.Api.Payment();
            pymnt.intent = "sale";
            pymnt.payer = payr;
            pymnt.transactions = transactions;

            try
            {
                //getting context from the paypal, basically we are sending the clientID and clientSecret key in this function 
                //to the get the context from the paypal API to make the payment for which we have created the object above.

                //Code for the configuration class is provided next

                // Basically, apiContext has a accesstoken which is sent by the paypal to authenticate the payment to facilitator account. An access token could be an alphanumeric string

                PayPal.Api.APIContext apiContext = ConfigurationPayment.GetAPIContext();

                // Create is a Payment class function which actually sends the payment details to the paypal API for the payment. The function is passed with the ApiContext which we received above.

                PayPal.Api.Payment createdPayment = pymnt.Create(apiContext);

                //if the createdPayment.State is "approved" it means the payment was successfull else not

                if (createdPayment.state.ToLower() != "approved")
                {
                    var order = db.Orders.Where(o => o.OderId == orderId).FirstOrDefault();
                    order.DeliveryStatus = Models.Extension.PaymentStatus.Delivered.ToString();
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();

                    return true;
                }
            }
            catch (PayPal.PayPalException ex)
            {
                _logger.Error("Error: " + ex.Message);
                return false;
            }
            return false;
        }
        /// <summary>
        /// Get List user shipping address
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        private List<SelectListItem> GetListUserAddress(int? selected = -1)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            if (db.UserAddresses.Any())
            {
                var listUserAddress = db.UserAddresses.ToList();
                items.Add(new SelectListItem { Text = null, Value = null });
                foreach (var dataMember in listUserAddress)
                {
                    var address = "";
                    address += !string.IsNullOrEmpty(dataMember.Address) ? dataMember.Address : "";
                    address += !string.IsNullOrEmpty(dataMember.City) ? ", " + dataMember.City : "";
                    address += !string.IsNullOrEmpty(dataMember.State) ? ", " + dataMember.State : "";
                    address += dataMember.ZipCode != null ? " " + dataMember.ZipCode : "";
                    address += !string.IsNullOrEmpty(dataMember.Country) ? ", " + dataMember.Country : "";

                    if (selected == dataMember.Id)
                    {

                        items.Add(new SelectListItem { Text = address, Value = dataMember.Id.ToString(), Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = address, Value = dataMember.Id.ToString(), Selected = false });
                    }
                }
                return items;
            }
            //else
            {
                items.Add(new SelectListItem { Text = null, Value = null, Selected = true });
                return items;
            }

        }


        private List<SelectListItem> GetListCountry(string selected = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (selected == "Singapore")
            {

                items.Add(new SelectListItem { Text = "Singapore", Value = "Singapore", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Singapore", Value = "Singapore", Selected = true });
            }
            if (selected == "Thailand")
            {

                items.Add(new SelectListItem { Text = "Thailand", Value = "Thailand", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Thailand", Value = "Thailand", Selected = false });
            }
            return items;
        }

        private List<SelectListItem> GetListCreditType(string selected = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();

            if (selected == "visa")
            {

                items.Add(new SelectListItem { Text = "Visa", Value = "visa", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Visa", Value = "visa", Selected = true });
            }
            if (selected == "mastercard")
            {

                items.Add(new SelectListItem { Text = "Mastercard", Value = "mastercard", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Mastercard", Value = "mastercard", Selected = false });
            }
            if (selected == "amex")
            {

                items.Add(new SelectListItem { Text = "Amex", Value = "amex", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Amex", Value = "amex", Selected = false });
            }
            if (selected == "discover")
            {

                items.Add(new SelectListItem { Text = "Discover", Value = "discover", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Discover", Value = "discover", Selected = false });
            }
            return items;
        }

        private List<SelectListItem> GetListMonthsCreditCard()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //months
            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                items.Add(new SelectListItem
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }
            return items;
        }
        private List<SelectListItem> GetListYearsCreditCard()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            //years
            for (int i = 0; i < 15; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                items.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
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
