﻿using System.Collections.Generic;
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
using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Utilities;
using System.Text;
using System.IO;
using System.Web;
using SBS_Ecommerce.Framework.Configurations;
using System.Web.Script.Serialization;
using SBS_Ecommerce.Models.Base;

namespace SBS_Ecommerce.Controllers
{
    [Authorize]
    public class OrdersController : BaseController
    {

        private const string PurchaseHistoryPath = "/Orders/PurchaseHistory.cshtml";
        private const string PurchaseProcessPath = "/Orders/PurchaseProcess.cshtml";
        private const string CheckoutAddressPath = "/Orders/CheckoutAddress.cshtml";
        private const string CheckoutShippingPath = "/Orders/CheckoutShiping.cshtml";
        private const string CheckoutPaymentPath = "/Orders/CheckoutPayment.cshtml";
        private const string DeliverySchedulerPath = "/Orders/DeliveryScheduler.cshtml";
        private const string CustomerNotificationEmailPath = "/Orders/CustomerNotificationEmail.cshtml";
        protected static readonly ILog _logger = LogManager.GetLogger(typeof(OrdersController));

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Models.Order order = await db.Orders.FindAsync(id);
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
        public ActionResult PurchaseProcess(string orderId)
        {
            var order = db.Orders.Find(orderId);
            var orderDetail = db.GetOrderDetails.Where(o => o.OrderId == orderId).ToList();
            var userShippingAddress = db.GetUserAddresses.Where(u => u.Id == order.ShippingAddressId).FirstOrDefault();
            var userBillingAddress = db.GetUserAddresses.Where(u => u.Id == order.BillingAddressId).FirstOrDefault();
            ViewBag.OrderDetail = orderDetail;
            ViewBag.UserShippingAddress = userShippingAddress;
            ViewBag.UserBillingAddress = userBillingAddress;

            var pathView = GetLayout() + PurchaseProcessPath;
            return View(pathView, order);
        }

        private async Task<int> DeleteOrder(string idOrder)
        {
            Models.Order order = await db.Orders.FindAsync(idOrder);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return await db.SaveChangesAsync();
        }

        private async Task<int> DeleteOrderDetail(string idOrder)
        {
            OrderDetail orderDetails = db.GetOrderDetails.Where(o => o.OrderId == idOrder).FirstOrDefault();
            db.OrderDetails.Remove(orderDetails);
            await db.SaveChangesAsync();
            return await db.SaveChangesAsync();
        }

        #region Checkout
        // GET: Orders/CheckoutSummary

        public ActionResult CheckoutSummary()
        {
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            if (string.IsNullOrEmpty(CurrentUser.Identity.Name))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = "/Orders/CheckoutShipping" });
            }
            else
            {
                return RedirectToAction("CheckoutShipping");
            }
        }
        [Authorize]
        public ActionResult CheckoutAddress()
        {
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Home");
            }
            var pathView = GetLayout() + CheckoutAddressPath;
            int id = GetIdUserCurrent();
            var userAddress = db.GetUserAddresses.Where(u => u.Uid == id).ToList();
            //ViewBag.GetListUserAddress = GetListUserAddress();

            return View(pathView, userAddress);
        }
        [Authorize]
        public ActionResult CheckoutShipping()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ConfigShippingDTO configShippingDTO = new ConfigShippingDTO();
            var shippingFee = db.GetConfigShippings.ToList();
            var pathView = GetLayout() + CheckoutShippingPath;
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            if (cart == null)
            {
                return RedirectToAction("Home", "Index");
            }

            configShippingDTO.ListConfigShipping = shippingFee;
            configShippingDTO.ListWeightBased = GetShippingFeeByWeight(cart.LstOrder);
            return View(pathView, configShippingDTO);
        }
        [HttpGet]
        public ActionResult CheckoutPayment()
        {
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Home");
            }
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
            ViewBag.CreditCardType = GetListCreditType();
            ViewBag.ExpireMonth = GetListMonthsCreditCard();
            ViewBag.ExpireYear = GetListYearsCreditCard();
            ViewBag.Bank = GetListBank();
           
            ViewBag.MoneyTransfer = cart.Total;
            var lstBank = SBSCommon.Instance.GetListBank(cId);
            var bankId = lstBank.FirstOrDefault() != null ? lstBank.FirstOrDefault().Bank_ID : -1000;
            ViewBag.BankAccount = GetListBankAccount(bankId);
            var configPaypal = db.GetConfigPaypals.FirstOrDefault();
            ViewBag.IsDisplayPayPal = true;
            if (configPaypal==null|| string.IsNullOrEmpty(configPaypal.ClientId) || string.IsNullOrEmpty(configPaypal.ClientSecret))
            {
                ViewBag.IsDisplayPayPal = false;
            }

            var pathView = GetLayout() + CheckoutPaymentPath;
            return View(pathView);
        }

        [HttpPost]
        public async Task<ActionResult> CheckoutPayment(PaymentModel paymentModel)
        {
            var pathView = GetLayout() + CheckoutPaymentPath;
            var company = SBSCommon.Instance.GetCompany();
            if (paymentModel == null)
            {
                ViewBag.CreditCardType = GetListCreditType();
                ViewBag.ExpireMonth = GetListMonthsCreditCard();
                ViewBag.ExpireYear = GetListYearsCreditCard();
                ViewBag.Bank = GetListBank();
                var banks = SBSCommon.Instance.GetListBank(cId);
                var bankID = banks.FirstOrDefault() != null ? banks.FirstOrDefault().Bank_ID : -1000;
                ViewBag.BankAccount = GetListBankAccount(bankID);
                return View(pathView, paymentModel);
            }
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
            //Get currency and country code from Api company
            paymentModel.CurrencyCode = company.Currency_Code;
            paymentModel.CountryCode = company.Country_Code;
            paymentModel.ShippingFee = cart.ShippingFee;
            paymentModel.shippingAddressId = cart.shippingAddressId;
            paymentModel.billingAddressId = cart.billingAddressId;
            //Check if payment by bank transfer
            if (paymentModel.PaymentMethod == (int)PaymentMethod.BankTranfer)
            {
                if (paymentModel.File != null)
                {
                    paymentModel.PaySlip = UploadPaySlip(paymentModel.File);
                }
                //get infor bank
                GetInfoBankTransfer(paymentModel);
            }

            var orderId = this.InsertDataOrder(cart, paymentModel);
            _logger.Info("Order create" + DateTime.Now + " with OrderID " + orderId);
            //If payment by bank transfer redirect to page status order
            if (paymentModel.PaymentMethod == (int)PaymentMethod.BankTranfer)
            {
                return RedirectToAction("PurchaseProcess", "Orders", new { orderId = orderId });
            }
            List<string> lstError = new List<string>();
            bool result = false;
            if (paymentModel.PaymentMethod == (int)PaymentMethod.CreditCard)
            {
                result = PaymentCreditCard(cart, paymentModel, orderId, ref lstError);
                if (result)
                {
                    Session["Cart"] = null;
                    return RedirectToAction("PurchaseProcess", "Orders", new { orderId = orderId });
                }
                else
                {
                    await DeleteOrder(orderId);
                    await DeleteOrderDetail(orderId);
                    ViewBag.Message = lstError;
                }
            }
            if (paymentModel.PaymentMethod == (int)PaymentMethod.CashOnDelivery)
            {
                Session["Cart"] = null;
                return RedirectToAction("PurchaseProcess", "Orders", new { orderId = orderId });
            }
            if (paymentModel.PaymentMethod == (int)PaymentMethod.Paypal)
            {
                return RedirectToAction("PaymentWithPaypal", "Orders", new { orderID = orderId, currencyCode = paymentModel.CurrencyCode });
            }
            ViewBag.CreditCardType = GetListCreditType();
            ViewBag.ExpireMonth = GetListMonthsCreditCard();
            ViewBag.ExpireYear = GetListYearsCreditCard();
            ViewBag.Bank = GetListBank();
            var lstBank = SBSCommon.Instance.GetListBank(cId);
            var bankId = lstBank.FirstOrDefault() != null ? lstBank.FirstOrDefault().Bank_ID : -1000;
            ViewBag.BankAccount = GetListBankAccount(bankId);
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
                var idUser = GetIdUserCurrent();
                var order = new Models.Order();
                var lstOrderDetail = new List<OrderDetail>();
                var idOrder = CommonUtil.GenerateOrderId();
                order.OrderId = idOrder;
                order.PaymentId = paymentModel.PaymentMethod;
                order.TotalAmount = cart.Total;
                order.CreatedAt = DateTime.Now;
                order.UpdatedAt = DateTime.Now;
                order.UId = GetIdUserCurrent();
                //order.ShippingStatus = (int)ShippingStatus.NotYetShipped;
                order.PaymentStatusId = (int)PaymentStatus.Pending;
                order.OrderStatus = (int)OrderStatus.Pending;
                order.Currency = paymentModel.CurrencyCode;
                order.CountProduct = cart.LstOrder.Count;
                order.ShippingFee = paymentModel.ShippingFee;
                order.ShippingAddressId = paymentModel.shippingAddressId;
                order.BillingAddressId = paymentModel.billingAddressId;
                order.Coupon = cart.Counpon;
                order.Discount = cart.Discount;
                order.ShippingProvider = cart.ShippingProvider;
                order.TimeShipping = cart.DateTimeShipping;
                order.Tax = cart.Tax;
                if (order.PaymentId == (int)PaymentMethod.BankTranfer)
                {
                    order.AccountCode = paymentModel.BankAccount;
                    order.AccountName = paymentModel.BankAccountName;
                    order.BankCode = paymentModel.Bank;
                    order.BankName = paymentModel.BankName;
                    order.Payslip = paymentModel.PaySlip;
                    order.MoneyTransfer = paymentModel.MoneyTranster;
                    //set default currency for order by bank transfer
                    order.Currency = "SGD";
                }

                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var detail in cart.LstOrder)
                {
                    var orderDetail = new OrderDetail();
                    orderDetail.OrderId = idOrder;
                    orderDetail.ProId = detail.Product.Product_ID;
                    orderDetail.Price = (detail.Product.Promotion_ID == -1 || detail.Product.IsApplyCoupon) ? detail.Product.Selling_Price : detail.Product.Promotion_Price.Value;
                    orderDetail.ProductName = detail.Product.Product_Name;
                    orderDetail.ProductImg = detail.Product.Small_Img;
                    orderDetail.Quantity = detail.Count;
                    orderDetail.Amount = SBSExtensions.ConvertMoneyDouble(orderDetail.Price * detail.Count);
                    orderDetail.OrderType = detail.Product.Allowable_PreOrder ? ((int)OrderType.PreOrder).ToString() : ((int)OrderType.Order).ToString();
                    // orderDetail.ShippingStatus = (int)PaymentStatus.Pending;
                    lstOrderDetail.Add(orderDetail);
                }
                db.OrderDetails.AddRange(lstOrderDetail);
                db.SaveChanges();
                // this.SendMailNotification(idOrder, idUser);

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

        private bool PaymentCreditCard(Models.Base.Cart cart, PaymentModel paymentModel, string orderId, ref List<string> lstError)
        {
            var idUser = GetIdUserCurrent();
            var user = db.Users.Find(idUser);
            //Now make a List of Item and add the above item to it
            //you can create as many items as you want and add to this list
            PayPal.Api.Item item = null;
            List<PayPal.Api.Item> itms = new List<PayPal.Api.Item>();
            var rateExchangeMonney = SBSCommon.Instance.GetRateExchange(paymentModel.CurrencyCode);
            double sumPriceProduct = 0;
            foreach (var order in cart.LstOrder)
            {
                item = new PayPal.Api.Item();
                double priceProduct = (order.Product.Promotion_ID == -1 || order.Product.IsApplyCoupon) ? order.Product.Selling_Price : order.Product.Promotion_Price.Value;
                item.name = order.Product.Product_Name;
                item.currency = "USD";
                item.price = SBSExtensions.ConvertMoneyString(priceProduct * rateExchangeMonney);
                item.quantity = order.Count.ToString();
                sumPriceProduct = sumPriceProduct + (double.Parse(item.price) * order.Count);
                itms.Add(item);
            }
            PayPal.Api.ItemList itemList = new PayPal.Api.ItemList();
            itemList.items = itms;

            //Address for the payment
            PayPal.Api.Address billingAddress = new PayPal.Api.Address();


            //Address for the payment
            var userAddress = db.GetUserAddresses.Where(a => (a.Uid == idUser)).FirstOrDefault();
            billingAddress.city = userAddress.City;
            billingAddress.country_code = paymentModel.CountryCode;
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
            details.shipping = SBSExtensions.ConvertMoneyString(cart.ShippingFee * rateExchangeMonney);
            details.subtotal = sumPriceProduct.ToString();
            details.tax = cart.Tax.ToString();

            // Specify your total payment amount and assign the details object
            PayPal.Api.Amount amnt = new PayPal.Api.Amount();
            amnt.currency = "USD";
            // Total = shipping tax + subtotal.
            amnt.total = (SBSExtensions.ConvertMoneyDouble(cart.Tax + float.Parse(details.shipping) + sumPriceProduct)).ToString();
            amnt.details = details;


            // Now make a trasaction object and assign the Amount object
            PayPal.Api.Transaction tran = new PayPal.Api.Transaction();
            tran.amount = amnt;
            tran.description = "Payment amount form page SBS Ecommerce.";
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

                if (createdPayment.state.ToLower() == "approved")
                {
                    var order = db.GetOrders.Where(o => o.OrderId == orderId).FirstOrDefault();
                    //order.ShippingStatus = (int)Models.Extension.ShippingStatus.NotYetShipped;
                    order.PaymentStatusId = (int)PaymentStatus.Paid;
                    order.OrderStatus = (int)OrderStatus.Pending;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    _logger.Info("Order Credit Card SUCCESS " + DateTime.Now + " with OrderID " + orderId);
                    // Send email notification 
                    SendMailNotification(orderId, idUser);
                    return true;
                }
                else
                {
                    _logger.Info("Order credit card FAILED " + DateTime.Now + " with OrderID " + orderId);

                    return false;
                }
            }
            catch (PayPal.PayPalException ex)
            {
                _logger.Error("Error PayPalException:" + orderId + " Message " + ex.Message);
                var paypalError = JsonConvert.DeserializeObject<PaypalApiErrorDTO>(((PayPal.ConnectionException)ex).Response);
                if (paypalError != null && paypalError.details != null)
                {
                    foreach (var itemError in paypalError.details[0])
                    {
                        lstError.Add(itemError.Value);
                    }
                }
                if (paypalError.details == null)
                {
                    lstError = new List<string>();
                    lstError.Add(paypalError.message);
                }
                if (lstError.Where(err => err.Contains("Expiration date")).Any())
                {
                    lstError = new List<string>();
                    lstError.Add("Expiration date cannot be in the past.");
                }
                if (lstError.Where(err => err.Contains("card type")).Any())
                {
                    lstError = new List<string>();
                    lstError.Add("Length must be 3 or 4, depending on card type.");
                }
                lstError.Add("Please try again!");
                return false;
            }
        }

        public async Task<ActionResult> PaymentWithPaypal(string orderID, string currencyCode)
        {
            //getting the apiContext as earlier
            PayPal.Api.APIContext apiContext = ConfigurationPayment.GetAPIContext();
            var idUser = GetIdUserCurrent();
            try
            {
                string payerId = Request.Params["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist

                    //it is returned by the create function call of the payment class

                    // Creating a payment

                    // baseURL is the url on which paypal sendsback the data.

                    // So we have provided URL of this controller only

                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/" + Request.ApplicationPath + "/Orders/PaymentWithPayPal?orderID=" + orderID;

                    //guid we are generating for storing the paymentID received in session

                    //after calling the create function and it is used in the payment execution

                    var guid = Convert.ToString((new Random()).Next(100000));

                    //CreatePayment function gives us the payment approval url

                    //on which payer is redirected for paypal acccount payment

                    var createdPayment = this.CreatePayment(apiContext, baseURI + "&guid=" + guid, orderID, currencyCode);
                    if (createdPayment == null)
                    {
                        RedirectToAction("Index", "Home");
                    }

                    //get links returned from paypal in response to Create function call

                    var links = createdPayment.links.GetEnumerator();

                    string paypalRedirectUrl = null;

                    while (links.MoveNext())
                    {
                        PayPal.Api.Links lnk = links.Current;

                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment
                            paypalRedirectUrl = lnk.href;
                        }
                    }

                    // saving the paymentID in the key guid
                    Session.Add(guid, createdPayment.id);

                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This section is executed when we have received all the payments parameters

                    // from the previous call to the function Create

                    // Executing a payment

                    var guid = Request.Params["guid"];

                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    if (executedPayment.state.ToLower() == "approved")
                    {
                        var order = db.GetOrders.Where(o => o.OrderId == orderID).FirstOrDefault();
                        //  order.ShippingStatus = (int)Models.Extension.ShippingStatus.NotYetShipped;
                        order.PaymentStatusId = (int)PaymentStatus.Paid;
                        order.OrderStatus = (int)OrderStatus.Pending;

                        db.Entry(order).State = EntityState.Modified;
                        db.SaveChanges();
                        //Send email notification to customer
                        SendMailNotification(orderID, idUser);
                        _logger.Info("Order redirect to Paypal SUCCESS " + DateTime.Now + " with " + orderID);
                        Session["Cart"] = null;
                        return RedirectToAction("PurchaseProcess", "Orders", new { orderId = orderID });
                    }
                    else
                    {
                        await DeleteOrder(orderID);
                        await DeleteOrderDetail(orderID);
                        _logger.Error("Order redirect to Paypal FAILED " + DateTime.Now + " with orderID " + orderID);
                    }
                }
            }
            catch (Exception ex)
            {
                //await DeleteOrder(orderID);
                //await DeleteOrderDetail(orderID);
                _logger.Error("Error PaymentWithPaypal " + ex.Message);
            }

            return RedirectToAction("Index", "Home");
        }

        private PayPal.Api.Payment payment;
        private PayPal.Api.Payment ExecutePayment(PayPal.Api.APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PayPal.Api.PaymentExecution() { payer_id = payerId };
            this.payment = new PayPal.Api.Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private PayPal.Api.Payment CreatePayment(PayPal.Api.APIContext apiContext, string redirectUrl, string orderID, string currencyCode)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();

            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                return null;
            }

            //similar to credit card create itemlist and add item objects to it
            var itemList = new PayPal.Api.ItemList() { items = new List<PayPal.Api.Item>() };

            PayPal.Api.Item item = null;
            List<PayPal.Api.Item> itms = new List<PayPal.Api.Item>();
            double cartAmount = 0;
            foreach (var order in cart.LstOrder)
            {
                item = new PayPal.Api.Item();
                double priceProduct = (order.Product.Promotion_ID == -1 || order.Product.IsApplyCoupon) ? order.Product.Selling_Price : order.Product.Promotion_Price.Value;
                item.name = order.Product.Product_Name;
                item.currency = currencyCode;
                item.price = Math.Round(priceProduct, 2).ToString();
                item.quantity = order.Count.ToString();
                cartAmount += Math.Round(priceProduct, 2) * order.Count;
                itms.Add(item);
            }

            itemList.items = itms;
            cartAmount = Math.Round(cartAmount, 2);

            var payer = new PayPal.Api.Payer() { payment_method = "paypal" };

            // Configure Redirect Urls here with RedirectUrls object
            var redirUrls = new PayPal.Api.RedirectUrls()
            {
                cancel_url = redirectUrl,
                return_url = redirectUrl
            };

            // similar as we did for credit card, do here and create details object
            var details = new PayPal.Api.Details()
            {
                tax = cart.Tax.ToString(),
                shipping = cart.ShippingFee.ToString(),
                subtotal = (cartAmount).ToString(),
                shipping_discount = cart.Discount.ToString(),
            };

            // similar as we did for credit card, do here and create amount object
            if (cart.Discount < 0)
            {
                cart.Discount = 0;
            }
            var amount = new PayPal.Api.Amount()
            {
                currency = currencyCode,
                total = (cartAmount + cart.Tax + cart.ShippingFee - cart.Discount).ToString(), // Total must be equal to sum of shipping, tax and subtotal.
                details = details,
            };

            var transactionList = new List<PayPal.Api.Transaction>();

            transactionList.Add(new PayPal.Api.Transaction()
            {
                description = "Transaction description.",
                invoice_number = orderID,
                amount = amount,
                item_list = itemList
            });

            this.payment = new PayPal.Api.Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            // Create a payment using a APIContext
            return this.payment.Create(apiContext);

        }
        #region Upload file
        /// <summary>
        /// Upload payslip
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string UploadPaySlip(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string uniqueNameAvatar = CommonUtil.GetNameUnique() + file.FileName;
                    string path = Path.Combine(Server.MapPath(SBSConstants.LINK_UPLOAD_PAYSLIP),
                                               Path.GetFileName(uniqueNameAvatar));
                    file.SaveAs(path);
                    return SBSConstants.LINK_UPLOAD_PAYSLIP + uniqueNameAvatar;

                }
                catch (Exception ex)
                {
                    return string.Empty;
                    throw ex;
                }
            else
            {
                return string.Empty;
            }
        }
        #endregion
        #region Send mail
        public ActionResult CustomerNotificationEmail()
        {
            //var pathView = GetLayout() + CustomerNotificationEmailPath;
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
            var emailAccount = db.GetEmailAccounts.FirstOrDefault();

            //Order
            var order = db.Orders.Find(orderId);
            //Order model email
            var emailModel = new EmailNotificationDTO();

            var mailUtil = new EmailUtil(emailAccount.Email, emailAccount.DisplayName,
                emailAccount.Password, emailAccount.Host, emailAccount.Port);
            var nameCustomer = customer.FirstName + " " + customer.LastName;

            var lstOrderDetail = db.GetOrderDetails.Where(o => o.OrderId == orderId).ToList();
            var lstOrderDetailModel = AutoMapper.Mapper.Map<List<OrderDetail>, List<OrderDetailDTO>>(lstOrderDetail);

            //Company
            var company = SBSCommon.Instance.GetCompany();

            emailModel.ListOrderEmail = lstOrderDetailModel;
            emailModel.User = customer;
            emailModel.Order = order;
            emailModel.OrderStatus = this.GetOrderStatus(order);
            emailModel.Company = company;

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
        private string GetOrderStatus(Models.Order order)
        {
            if (order.OrderStatus == (int)OrderStatus.Cancelled)
            {
                return "Cancelled";
            }
            if (order.OrderStatus == (int)OrderStatus.Completed)
            {
                return "Completed";
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
                var listUserAddress = db.GetUserAddresses.ToList();
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

        private List<SelectListItem> GetListBank()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var lstBank = SBSCommon.Instance.GetListBank(cId);
            //years

            foreach (var item in lstBank)
            {
                items.Add(new SelectListItem
                {
                    Text = item.Bank_Name,
                    Value = item.Bank_Code,
                });
            }
            return items;
        }
        /// <summary>
        /// Get list bank account of admin
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetListBankAccount(int bankId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            var lstBankAccount = SBSCommon.Instance.GetListBankAccount(cId);
            var bankAccount = lstBankAccount.Where(b => b.Bank_ID == bankId).FirstOrDefault();
            if (bankAccount != null)
            {
                items.Add(new SelectListItem
                {
                    Text = "Account name: " + bankAccount.Account_Name + " - Account code: " + bankAccount.Account_Code,
                    Value = bankAccount.Account_Code,
                });
            }
            else
            {
                items.Add(new SelectListItem
                {
                    Text = null,
                    Value = null,
                });
            }

            return items;
        }
        [HttpGet]
        public JsonResult GetListBankAcountByIdBank(string bankCode)
        {
            if (string.IsNullOrEmpty(bankCode))
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            StringBuilder selectBankAcount = new StringBuilder();
            var lstBank = SBSCommon.Instance.GetListBank(cId);
            var bank = lstBank.Where(b => b.Bank_Code == bankCode).FirstOrDefault();
            if (bank != null)
            {
                var lstBankAccount = SBSCommon.Instance.GetListBankAccount(cId);
                lstBankAccount = lstBankAccount.Where(b => b.Bank_ID == bank.Bank_ID).ToList();
                selectBankAcount.Append("<select class='form-control valid' id='BankAcount' name='BankAcount'>");
                foreach (var item in lstBankAccount)
                {
                    selectBankAcount.Append("<option value='" + item.Account_Code + "'>Account name: " + item.Account_Name + " - Account code: " + item.Account_Code + "</option>");
                }
                selectBankAcount.Append("</select>");
                return Json(selectBankAcount.ToString(), JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
        }

        private void GetInfoBankTransfer(PaymentModel paymentModel)
        {
            var lstBank = SBSCommon.Instance.GetListBank(cId);
            var lstBankAccount = SBSCommon.Instance.GetListBankAccount(cId);
            var bank = lstBank.Where(b => b.Bank_Code == paymentModel.Bank).FirstOrDefault();
            var bankAccount = lstBankAccount.Where(b => b.Account_Code == paymentModel.BankAccount).FirstOrDefault();

            paymentModel.BankName = bank != null ? bank.Bank_Name : string.Empty;
            paymentModel.BankAccountName = bankAccount != null ? bankAccount.Account_Name : string.Empty;
        }
        [HttpPost]
        public ActionResult ChooseShippingPayment(ConfigShippingDTO configShippingDTO, string shippingMethod, string timeSlot, string dateTimeShipping)
        {
            if (shippingMethod == ((int)ShippingMethod.NoShipping).ToString())
            {
                Models.Base.Cart cart = (Models.Base.Cart)Session["Cart"];
                if (cart == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                cart.ShippingFee = 0;
                double sumPriceProduct = 0;
                foreach (var order in cart.LstOrder)
                {
                    double priceProduct = (order.Product.Promotion_ID == -1 || order.Product.IsApplyCoupon) ? order.Product.Selling_Price : order.Product.Promotion_Price.Value;
                    sumPriceProduct = sumPriceProduct + (priceProduct * order.Count);
                }
                sumPriceProduct = Math.Round(sumPriceProduct, 2);
                cart.Total = Math.Round(sumPriceProduct + cart.Tax);
                Session["Cart"] = cart;

                return RedirectToAction("CheckoutPayment");
            }
            else
            {
                var weightBase = configShippingDTO.ListWeightBased.Where(w => w.Id.ToString() == shippingMethod).FirstOrDefault();

                Models.Base.Cart cart = (Models.Base.Cart)Session["Cart"];
                cart.ShippingFee = weightBase.Rate;
                double sumPriceProduct = 0;
                foreach (var order in cart.LstOrder)
                {
                    double priceProduct = (order.Product.Promotion_ID == -1 || order.Product.IsApplyCoupon) ? order.Product.Selling_Price : order.Product.Promotion_Price.Value;
                    sumPriceProduct = sumPriceProduct + (priceProduct * order.Count);
                }
                if (!string.IsNullOrEmpty(timeSlot))
                {
                    cart.ShippingFee = cart.ShippingFee + int.Parse(timeSlot);
                }
                cart.DateTimeShipping = dateTimeShipping;
                cart.Total = Math.Round(sumPriceProduct, 2);
                cart.ShippingProvider = weightBase.DeliveryCompany;
                Session["Cart"] = cart;
                return RedirectToAction("CheckoutAddress");
            }
        }
        public List<WeightBased> GetShippingFeeByWeight(List<Models.Base.Order> lstOrder)
        {
            if (lstOrder == null)
            {
                return new List<WeightBased>();
            }
            List<WeightBased> lstWeightBased = new List<WeightBased>();
            foreach (var item in lstOrder)
            {
                var weightBased = db.GetWeightBaseds.AsEnumerable().Where(w => w.Min <= item.Product.Weight * item.Count && w.Max >= item.Product.Weight * item.Count
                && !string.IsNullOrEmpty(item.Product.Weight_UOM) && w.UnitOfMass != null && w.UnitOfMass.ToLower() == item.Product.Weight_UOM.ToLower().ToString()).ToList();
                if (weightBased != null)
                {
                    lstWeightBased.AddRange(weightBased);
                }
            }
            if (lstWeightBased != null && lstWeightBased.Count > 0)
            {
                lstWeightBased = lstWeightBased.GroupBy(w => w.DeliveryCompany).Select(g => new WeightBased
                {
                    Rate = g.Sum(c => c.Rate),
                    DeliveryCompany = g.First().DeliveryCompany
                }).ToList();
                for (int i = 0; i < lstWeightBased.Count; i++)
                {
                    lstWeightBased[i].Id = i;
                }
            }

            return lstWeightBased;
        }

        public ActionResult DeliveryScheduler()
        {
            var deliverySchedule = db.GetDeliverySchedulers.ToList();
            var configShipping = db.GetConfigDeliveryDays.FirstOrDefault();
            List<DeliveryDateDTO> lstDeliveryDateDTO = new List<DeliveryDateDTO>();
            DeliveryDateDTO deliveryDateDTO = null;
            for (int i = 0; i < configShipping.NumOfDeliveryDay; i++)
            {
                deliveryDateDTO = new DeliveryDateDTO();
                deliveryDateDTO.DateTime = DateTime.Today.AddDays(i).ToString("MM/dd/yyyy");
                deliveryDateDTO.TimeSlot = new List<TimeSlot>();
                foreach (var item in deliverySchedule)
                {
                    TimeSlot timeSlot = new TimeSlot();
                    timeSlot.NameTimeSlot = item.TimeSlot;
                    timeSlot.Money = item.Rate.ToString();
                    deliveryDateDTO.TimeSlot.Add(timeSlot);
                }
                lstDeliveryDateDTO.Add(deliveryDateDTO);

            }
            var pathView = GetLayout() + DeliverySchedulerPath;
            return View(pathView, lstDeliveryDateDTO);
        }

        /// <summary>
        /// disposing
        /// </summary>
        /// <param name="disposing"></param>
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
