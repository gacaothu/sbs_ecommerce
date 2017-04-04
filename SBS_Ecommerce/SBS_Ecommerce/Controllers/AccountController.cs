using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Framework.Utilities;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Collections.Generic;
using SBS_Ecommerce.Models.DTOs;
using AutoMapper;
using System.Data.Entity.Migrations;
using System.Security.Claims;
using Microsoft.Owin;
using Facebook;
using System.IO;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;

namespace SBS_Ecommerce.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        private const string ExternalLoginConfirmationPath = "/Account/ExternalLoginConfirmation.cshtml";
        private const string AddShippingAddressPath = "/Account/AddShippingAddress.cshtml";
        private const string AddShippingAddressCheckOutPath = "/Account/AddShippingAddressCheckOut.cshtml";
        private const string EditShippingAddressPath = "/Account/EditShippingAddress.cshtml";
        private const string ChangeAvatarPath = "/Account/ChangeAvatar.cshtml";
        private const string ListShippingAddressPath = "/Account/ListShippingAddress.cshtml";
        private const string LoginPath = "/Account/Login.cshtml";
        private const string InforCustomerPath = "/Account/InforCustomer.cshtml";
        private const string OrderHistoryPath = "/Account/OrderHistory.cshtml";
        private const string ProductReviewPath = "/Account/ProductReviews.cshtml";

        private const string AddressAddPath = "/Account/AddressAdd.cshtml";
        private const string ProfilePath = "/Account/ViewProfile.cshtml";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private SBS_Entities db = new SBS_Entities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            var pathView = GetLayout() + LoginPath;
            ViewBag.ReturnUrl = returnUrl;
            return View(pathView);
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.Failure:
                default:
                    ViewBag.Message = "Invalid login attempt.";
                    var pathView = GetLayout() + LoginPath;
                    ViewBag.ReturnUrl = returnUrl;
                    return View(pathView, model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        //[AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Register(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    var userModel = new User();
                    userModel.Email = model.Email;
                    userModel.Password = PasswordUtil.Encrypt(model.Password);
                    userModel.CreatedAt = DateTime.Now;
                    userModel.UpdatedAt = DateTime.Now;
                    userModel.Status = "1";
                    userModel.UserType = "N";
                    db.Users.Add(userModel);
                    await db.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }
                if (result.Errors.Where(e => e.ToString().Contains("is already taken")).Any())
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.ToString().Contains("Email"))
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                    // ModelState.AddModelError("", "Email is already taken.");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            var layout = GetLayout();
            var pathView = GetLayout() + LoginPath;
            return View(pathView, model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = "/Home" }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();


            var identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
            var accessToken = identity.FindFirstValue("FacebookAccessToken");
            var fb = new FacebookClient(accessToken);
            dynamic myInfo = fb.Get("/me?fields=email,first_name,last_name,gender");

            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            if (result == SignInStatus.Failure)
            {
                string email = myInfo["email"];
                string first_name = myInfo["first_name"];
                string last_name = myInfo["last_name"];
                string gender = myInfo["gender"];

                var user = new ApplicationUser { UserName = email, Email = email };
                var resultLogin = await UserManager.CreateAsync(user);

                if (resultLogin.Succeeded)
                {
                    var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                    resultLogin = await UserManager.AddLoginAsync(user.Id, info.Login);
                    var userModel = new User();
                    userModel.Email = email;
                    if (gender == "male")
                    {
                        userModel.Gender = "M";
                    }
                    if (gender == "female")
                    {
                        userModel.Gender = "F";
                    }
                    userModel.FirstName = first_name;
                    userModel.LastName = last_name;
                    userModel.FacebookId = user.Id;
                    userModel.CreatedAt = DateTime.Now;
                    userModel.UpdatedAt = DateTime.Now;
                    userModel.Status = "1";
                    userModel.UserType = "N"; // User typle is normal
                    db.Users.Add(userModel);
                    await db.SaveChangesAsync();

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // Get the information about the user from the external login provider
        //            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //            if (info == null)
        //            {
        //                return View("ExternalLoginFailure");
        //            }
        //            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //            var result = await UserManager.CreateAsync(user);
        //            if (result.Succeeded)
        //            {
        //                result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //                var userModel = new User();
        //                userModel.Email = model.Email;
        //                userModel.FirstName = info.DefaultUserName;
        //                userModel.FacebookId = user.Id;
        //                userModel.CreatedAt = DateTime.Now;
        //                userModel.UpdatedAt = DateTime.Now;
        //                userModel.Status = "1";
        //                userModel.UserType = "N"; // User typle is normal
        //                db.Users.Add(userModel);
        //                await db.SaveChangesAsync();
        //                if (result.Succeeded)
        //                {
        //                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                    return RedirectToLocal(returnUrl);
        //                }
        //            }
        //            AddErrors(result);
        //        }

        //        ViewBag.ReturnUrl = returnUrl;
        //        return View(model);
        //    }
        //    catch (DbEntityValidationException e)
        //    {
        //        foreach (var eve in e.EntityValidationErrors)
        //        {
        //            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
        //                eve.Entry.Entity.GetType().Name, eve.Entry.State);
        //            foreach (var ve in eve.ValidationErrors)
        //            {
        //                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
        //                    ve.PropertyName, ve.ErrorMessage);
        //            }
        //        }
        //        throw;
        //    }
        //}

        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        public ActionResult ListShippingAddress()
        {
            var idUser = GetIdUserCurrent();
            var lstUserAddress = db.UserAddresses.Where(u => u.Uid == idUser).ToList();

            var model = Mapper.Map<List<UserAddress>, List<ShippingAddressDTO>>(lstUserAddress);
            var pathView = GetLayout() + ListShippingAddressPath;
            return View(pathView, model);
        }

        [Authorize]
        public ActionResult InforCustomer()
        {
            int id = GetIdUserCurrent();
            if (id == -1)
            {
                return RedirectToAction("Login");
            }
            User user = db.Users.Where(u => u.Id == id).FirstOrDefault();
            var model = Mapper.Map<User, UserDTO>(user);
            var pathView = GetLayout() + InforCustomerPath;
            return View(pathView, model);
        }
        [HttpPost]
        public async Task<ActionResult> InforCustomer(UserDTO userDTO)
        {
            int id = GetIdUserCurrent();
            if (id == -1)
            {
                return RedirectToAction("Login");
            }

            var pathView = GetLayout() + InforCustomerPath;
            AspNetUser user = db.AspNetUsers.Where(u => (u.Email == userDTO.Email || u.UserName == userDTO.Email)).FirstOrDefault();
            if (user != null && userDTO.Email != CurrentUser.Identity.Name)
            {
                ModelState.AddModelError("", "Emails is exists.");
                return View(pathView, userDTO);
            }
            try
            {
                if (ModelState.IsValid)
                {
                    User model = Mapper.Map<UserDTO, User>(userDTO);
                    model.UpdatedAt = DateTime.Now;
                    model.UserType = "N";
                    model.Status = "1";
                    db.Entry(model).State = EntityState.Modified;
                    db.Entry(model).Property("PaymentId").IsModified = false;
                    db.Entry(model).Property("Password").IsModified = false;
                    db.Entry(model).Property("FacebookId").IsModified = false;
                    db.Entry(model).Property("CreatedAt").IsModified = false;
                    db.Entry(model).Property("Avatar").IsModified = false;

                    var userLogin = db.AspNetUsers.Find(User.Identity.GetUserId());
                    userLogin.Email = userDTO.Email;
                    userLogin.UserName = userDTO.Email;
                    db.Entry(userLogin).State = EntityState.Modified;

                    db.SaveChanges();

                    ApplicationUser modelCurrent = UserManager.FindById(User.Identity.GetUserId());
                    modelCurrent.Email = userDTO.Email;
                    modelCurrent.UserName = userDTO.Email;
                    //IdentityResult result = UserManager.Update(modelCurrent);

                    await UpdateCurrent(modelCurrent);
                }

                return View(pathView, userDTO);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
        public ActionResult OrderHistory(string productName, string dateFrom, string dateTo, string orderStatus)
        {
            int id = GetIdUserCurrent();
            if (id == -1)
            {
                return RedirectToAction("Login");
            }
            var order = db.Orders.Where(u => u.UId == id).ToList();
            var orderDetail = db.OrderDetails.ToList();
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.ProductName = productName;
            ViewBag.OrderStatus = this.GetListOrderStatus(orderStatus);
            if (!string.IsNullOrEmpty(productName))
            {
                var newOrder = (from od in db.OrderDetails
                                join o in db.Orders on od.OrderId equals o.OderId
                                where od.ProductName.Contains(productName)
                                where o.UId == id
                                select o).ToList();
                order = newOrder;
            }
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                order = order.Where(o => (o.CreatedAt >= DateTime.Parse(dateFrom) && o.CreatedAt <= DateTime.Parse(dateTo))).ToList();
            }
            if (string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                order = order.Where(o => o.CreatedAt <= DateTime.Parse(dateTo)).ToList();
            }
            if (!string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                order = order.Where(o => o.CreatedAt >= DateTime.Parse(dateFrom)).ToList();
            }
            if (!string.IsNullOrEmpty(orderStatus) && !"All".Equals(orderStatus))
            {
                order = order.Where(o => o.DeliveryStatus.Contains(orderStatus)).ToList();
            }

            var model = Mapper.Map<List<Order>, List<OrderDTO>>(order);
            foreach (var item in model)
            {
                item.PaymentName = db.Payments.Find(item.PaymentId).Name;
                item.OrderDetails = db.OrderDetails.Where(o => o.OrderId == item.OderId).ToList();
                item.DeliveryStatus = this.GetStatusByCode(item.DeliveryStatus);
            }

            var pathView = GetLayout() + OrderHistoryPath;
            ViewBag.OrderStatus = GetListOrderStatus();
            return View(pathView, model);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        //Update Profile

        public JsonResult GetShippingAddressById(int? id)
        {
            if (id == null)
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            UserAddress userAddress = db.UserAddresses.Where(a => a.Id == id).FirstOrDefault();
            if (userAddress != null)
            {
                return Json(userAddress, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "Not found" }, JsonRequestBehavior.AllowGet);
        }
        #region Page profile
        /// <summary>
        /// Return screen add shipping address page checkout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddShippingAddress()
        {
            var pathView = GetLayout() + AddShippingAddressPath;
            ShippingAddressDTO userAddress = new ShippingAddressDTO();
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Return screen add shipping address page checkout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddShippingAddressCheckOut()
        {
            var pathView = GetLayout() + AddShippingAddressCheckOutPath;
            ShippingAddressDTO userAddress = new ShippingAddressDTO();
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddShippingAddress(ShippingAddressDTO userAddress)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<ShippingAddressDTO, UserAddress>(userAddress);

                    model.Uid = GetIdUserCurrent();
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.AddressType = "1";
                    db.UserAddresses.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("ListShippingAddress");
                }
                ViewBag.Country = GetListCountry(userAddress.Country);
                var pathView = GetLayout() + AddShippingAddressPath;
                return View(pathView, userAddress);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddShippingAddressCheckOut(ShippingAddressDTO userAddress)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<ShippingAddressDTO, UserAddress>(userAddress);

                    model.Uid = GetIdUserCurrent();
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    model.AddressType = "1";
                    db.UserAddresses.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("CheckoutAddress", "Orders");
                }
                ViewBag.Country = GetListCountry(userAddress.Country);
                var pathView = GetLayout() + AddShippingAddressPath;
                return View(pathView, userAddress);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Return screen add shipping address page checkout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditShippingAddress(int id)
        {
            var shippingAddress = db.UserAddresses.Find(id);
            var model = Mapper.Map<UserAddress, ShippingAddressDTO>(shippingAddress);

            var pathView = GetLayout() + EditShippingAddressPath;
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, model);
        }
        /// <summary>
        /// Function edit shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditShippingAddress(ShippingAddressDTO userAddress)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<ShippingAddressDTO, UserAddress>(userAddress);
                    model.UpdatedAt = DateTime.Now;

                    db.Entry(model).State = EntityState.Modified;
                    db.Entry(model).Property("CreatedAt").IsModified = false;
                    db.Entry(model).Property("AddressType").IsModified = false;
                    db.Entry(model).Property("Uid").IsModified = false;
                    db.SaveChanges();
                    return RedirectToAction("ListShippingAddress");

                }
                ViewBag.Country = GetListCountry(userAddress.Country);
                var pathViewEditShippingAddressPath = GetLayout() + EditShippingAddressPath;
                return View(pathViewEditShippingAddressPath);
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }

        public ActionResult AddressDelete(int addressId)
        {
            var customer = db.UserAddresses;

            //find address (ensure that it belongs to the current customer)
            var address = customer.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                db.UserAddresses.Remove(address);
                db.SaveChangesAsync();
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("ListShippingAddress"),
            });
        }
        [HttpGet]
        public ActionResult ChooseAddressShipping(int addressId)
        {
            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CheckoutPayment"),
            },JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ChangeAvatar()
        {
            var pathView = GetLayout() + ChangeAvatarPath;
            var user = db.Users.Find(GetIdUserCurrent());

            if (user != null && !string.IsNullOrEmpty(user.Avatar))
            {
                ViewBag.UrlAvartar = user.Avatar;
            }
            else
            {
                ViewBag.UrlAvartar = SBSConstants.LINK_UPLOAD_AVATAR_DEFAULT;
                ViewBag.NoAvatar = true;
            }
            return View(pathView);
        }

        [HttpGet]
        public ActionResult RemoveAvatar()
        {
            var pathView = GetLayout() + ChangeAvatarPath;
            var user = db.Users.Find(GetIdUserCurrent());
            user.Avatar = null;
            user.UpdatedAt = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                redirect = Url.RouteUrl("ChangeAvatar"),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase file)
        {
            var user = db.Users.Find(GetIdUserCurrent());

            if (file != null && file.ContentLength > 0)
                try
                {
                    string uniqueNameAvatar = SBSExtensions.GetNameUnique() + file.FileName;
                    string path = Path.Combine(Server.MapPath(SBSConstants.LINK_UPLOAD_AVATAR),
                                               Path.GetFileName(uniqueNameAvatar));
                    file.SaveAs(path);
                    user.Avatar = SBSConstants.LINK_UPLOAD_AVATAR + uniqueNameAvatar;
                    user.UpdatedAt = DateTime.Now;
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ChangeAvatar");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            else
            {
                return RedirectToAction("ChangeAvatar");
            }
        }
        public string GetNameByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
            SBS_Entities db = new SBS_Entities();
            var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(user.FirstName) && string.IsNullOrEmpty(user.LastName))
            {
                return user.Email;
            }
            return user.FirstName + " " + user.LastName;
        }

        #endregion

        #region Page ProductReview
        [Authorize]
        public ActionResult ProductReviews()
        {
            var id = GetIdUserCurrent();
            var productReviews = db.ProductReviews.Where(p => p.UId == id).ToList();
            var productReviewModel = Mapper.Map<List<ProductReview>, List<ProductReviewDTO>>(productReviews);
            var pathView = GetLayout() + ProductReviewPath;
            return View(pathView, productReviewModel);
        }


        #endregion

        #region Validation
        public JsonResult CheckExistsEmail(string email)
        {
            if (email == null)
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            User user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return Json(new { status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region GetListItemDropdownList

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

        private string GetStatusByCode(string code)
        {
            if (code == "0")
            {
                return "Pending";
            }
            if (code == "1")
            {
                return "Processed";
            }
            if (code == "2")
            {
                return "Delivered";
            }
            if (code == "3")
            {
                return "Canceled";
            }
            return null;
        }

        private List<SelectListItem> GetListOrderStatus(string status = "")
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "All", Value = null, Selected = true });

            if (status == "0")
            {

                items.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = "Pending", Value = "0", Selected = true });
            }
            if (status == "1")
            {

                items.Add(new SelectListItem { Text = "Processed", Value = "1", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Processed", Value = "1", Selected = false });
            }
            if (status == "2")
            {

                items.Add(new SelectListItem { Text = "Delivered", Value = "2", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Delivered", Value = "2", Selected = false });
            }
            if (status == "3")
            {

                items.Add(new SelectListItem { Text = "Canceled", Value = "3", Selected = false }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = "Canceled", Value = "3", Selected = false });
            }
            return items;
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task UpdateCurrent(ApplicationUser user)
        {
            await SignInAsync(user, isPersistent: false);
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}