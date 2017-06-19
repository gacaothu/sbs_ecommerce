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
using Facebook;
using System.IO;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Models.Extension;
using PagedList;
using System.Dynamic;
using System.Net;
using SBS_Ecommerce.Framework.Repositories;

namespace SBS_Ecommerce.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private const string ExternalLoginConfirmationPath = "/Account/ExternalLoginConfirmation.cshtml";
        private const string AddShippingAddressPath = "/Account/AddShippingAddress.cshtml";
        private const string AddBillinggAddressPath = "/Account/AddBillingAddress.cshtml";
        private const string AddShippingAddressCheckOutPath = "/Account/AddShippingAddressCheckOut.cshtml";
        private const string AddBillingAddressCheckOutPath = "/Account/AddBillingAddressCheckOut.cshtml";
        private const string EditShippingAddressPath = "/Account/EditShippingAddress.cshtml";
        private const string EditBillingAddressPath = "/Account/EditBillingAddress.cshtml";
        private const string ChangeAvatarPath = "/Account/ChangeAvatar.cshtml";
        private const string ListShippingAddressPath = "/Account/ListShippingAddress.cshtml";
        private const string ListBillingAddressPath = "/Account/ListBillingAddress.cshtml";
        private const string LoginPath = "/Account/Login.cshtml";
        private const string ForgotPasswordPath = "/Account/ForgotPassword.cshtml";
        private const string ForgotPasswordConfirmationPath = "/Account/ForgotPasswordConfirmation.cshtml";
        private const string ResetPasswordPath = "/Account/ResetPassword.cshtml";
        private const string ResetPasswordConfirmPath = "/Account/ResetPasswordConfirm.cshtml";
        private const string InforCustomerPath = "/Account/InforCustomer.cshtml";
        private const string OrderHistoryPath = "/Account/OrderHistory.cshtml";
        private const string ProductReviewPath = "/Account/ProductReviews.cshtml";
        private const string CreditPointPath = "/Account/Creditpoint.cshtml";
        private const string ClassName = nameof(HomeController);

        private const string AddressAddPath = "/Account/AddressAdd.cshtml";
        private const string ProfilePath = "/Account/ViewProfile.cshtml";
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private SBSUnitWork unitWork;

        public AccountController()
        {
            unitWork = new SBSUnitWork();
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
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            var pathView = GetLayout() + LoginPath;
            ViewBag.ReturnUrl = returnUrl;
            return View(pathView, loginViewModel);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword(string returnUrl)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            var pathView = GetLayout() + ForgotPasswordPath;
            ViewBag.ReturnUrl = returnUrl;
            return View(pathView, loginViewModel);
        }
        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(LoginViewModel model)
        {
            var userDb = await unitWork.Repository<User>().GetAsync(u => u.Email == model.Email);
            var user = await UserManager.FindByNameAsync(model.Email);
            var emailAccount = unitWork.Repository<EmailAccount>().GetAll().FirstOrDefault();
            //var userDb = db.GetUsers.Where(u => u.Email == model.Email).FirstOrDefault();

            var pathView = GetLayout() + ForgotPasswordConfirmationPath;
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return View(pathView);
            }

            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
            //var emailAccount = db.GetEmailAccounts.FirstOrDefault();
            var mailUtil = new EmailUtil(emailAccount.Email, emailAccount.DisplayName,
            emailAccount.Password, emailAccount.Host, emailAccount.Port);
            string fullName = userDb.FirstName + userDb.LastName;
            mailUtil.SendEmail(model.Email, user.UserName, "Password recovery", "Dear "+ fullName + ",<br /><br />  You recently requested to reset your password for your account.<br /> Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a><br />  <br /><br /> Thanks,<br /> " + emailAccount.DisplayName, true);

            // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            return RedirectToAction("ForgotPasswordConfirmation", "Account");

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
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Gender");
            ModelState.Remove("ContactNum");
            ModelState.Remove("Birthday");
            ModelState.Remove("BirthdayYear");
            ModelState.Remove("BirthdayMonth");
            ModelState.Remove("BirthdayDay");
            ModelState.Remove("Phone");
            ModelState.Remove("Email");
            var pathView = GetLayout() + LoginPath;
            if (!ModelState.IsValid)
            {
                return View(pathView, model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.EmailLogin, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    {
                        //var userID = db.Users.Where(m => m.Email == model.EmailLogin).FirstOrDefault().Id;
                        var userID = unitWork.Repository<User>().Get(m => m.Email == model.EmailLogin).Id;
                        if (Session["Cart"] != null)
                        {
                            var cart = (Models.Base.Cart)Session["Cart"];

                            //Save The cart to cart of user
                            foreach (var item in cart.LstOrder)
                            {
                                //var cartDatabase = db.Carts.Where(m => m.UserId == userID && m.CompanyId == cId && m.ProID == item.Product.Product_ID).FirstOrDefault();
                                var cartDatabase = unitWork.Repository<Cart>().Get(m=>m.UserId == userID && m.CompanyId == cId && m.ProID==item.Product.Product_ID);
                                if (cartDatabase != null)
                                {
                                    cartDatabase.Quantity = cartDatabase.Quantity + item.Count;
                                }
                                else
                                {
                                    Cart cartOfDatabase = new Cart();
                                    cartOfDatabase.CompanyId = cId;
                                    cartOfDatabase.ProID = item.Product.Product_ID;
                                    cartOfDatabase.Quantity = item.Count;
                                    cartOfDatabase.IsPreOrder = item.Product.Allowable_PreOrder;
                                    cartOfDatabase.PreOrderNotice = item.Product.Delivery_Noted;
                                    cartOfDatabase.UserId = userID;

                                    //db.Carts.Add(cartOfDatabase);
                                    unitWork.Repository<Cart>().Add(cartOfDatabase);
                                }

                                //db.SaveChanges();
                                unitWork.SaveChanges();
                            }
                        }

                        //var lstCartofDatabse = db.Carts.Where(m => m.UserId == userID && m.CompanyId == cId);
                        var lstCartofDatabse = unitWork.Repository<Cart>().GetAll(m => m.UserId == userID && m.CompanyId == cId).ToList();
                        if (lstCartofDatabse != null && lstCartofDatabse.Count() > 0)
                        {
                            //Empty cart and add cart of database to the cart
                            Session["Cart"] = null;
                            foreach (var item in lstCartofDatabse)
                            {
                                AddCartWhenLogin(item.ProID, item.Quantity);
                            }
                        }
                        return RedirectToLocal(returnUrl);
                    }
                case SignInStatus.Failure:
                default:
                    ViewBag.Message = SBSMessages.MessageIncorrectLogin;

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
            var pathView = GetLayout() + LoginPath;

            if (model.year == 0 || model.month == 0 || model.date == 0)
            {
                ModelState.AddModelError("", "Birthday is invalid");
            }
            try
            {
                new DateTime(model.year, model.month, model.date).ToString();
            }
            catch (ArgumentOutOfRangeException)
            {
                ModelState.AddModelError("", "Birthday is invalid");
            }
            ModelState.Remove("EmailLogin");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = cId + model.Email, Email = model.Email, CompanyId = cId };

                UserManager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 1,
                    RequireNonLetterOrDigit = false,
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false,
                };
                UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = false
                };
                try
                {
                    //var userLogin = db.AspNetUsers.Where(u => u.Email == model.Email && u.CompanyId == cId).FirstOrDefault();
                    var userLogin = unitWork.Repository<AspNetUser>().Get(u => u.Email == model.Email && u.CompanyId == cId);
                    if (userLogin != null)
                    {
                        ModelState.AddModelError("Email", "Email is already taken");
                        return View(pathView, model);
                    }

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        try
                        {
                            var userModel = new User();
                            userModel.Email = model.Email;
                            userModel.Password = PasswordUtil.Encrypt(model.Password);
                            userModel.FirstName = model.FirstName;
                            userModel.LastName = model.LastName;
                            userModel.Gender = model.Gender;
                            userModel.Phone = model.Phone;
                            userModel.CreatedAt = DateTime.Now;
                            userModel.UpdatedAt = DateTime.Now;
                            userModel.Status = "1";
                            userModel.UserType = "N";
                            userModel.DOB = new DateTime(model.year, model.month, model.date).ToString();

                            // generate Member No
                            //string memberNo = GeneratorUtil.GenerateMemberNo();
                            //userModel.MemberNo = memberNo;
                            //userModel.CreditPoint = 0;

                            //db.Users.Add(userModel);
                            unitWork.Repository<User>().Add(userModel);

                            if (!string.IsNullOrEmpty(model.MemberNo))
                            {
                                //var refUser = db.GetUsers.Where(m => m.MemberNo == model.MemberNo).FirstOrDefault();
                                var refUser = await unitWork.Repository<User>().GetAsync(m => m.MemberNo == model.MemberNo);
                                if (refUser != null)
                                {
                                    refUser.CreditPoint = refUser.CreditPoint + 1;
                                    refUser.UpdatedAt = DateTime.Now;
                                    unitWork.Repository<User>().Update(refUser);
                                    //var entry = db.Entry(refUser);
                                    //entry.Property(e => e.CreditPoint).IsModified = true;
                                    //entry.Property(e => e.UpdatedAt).IsModified = true;
                                }
                            }
                            //await db.SaveChangesAsync();
                            await unitWork.SaveChangesAsync();
                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception)
                        {
                            //db.AspNetUsers.Remove(userLogin);
                            //await db.SaveChangesAsync();
                            unitWork.Repository<AspNetUser>().Delete(userLogin);
                            await unitWork.SaveChangesAsync();                            
                        }
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
                }

            }
            var listError = GetErrorListFromModelState(ModelState);
            // If we got this far, something failed, redisplay form

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
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            var pathView = GetLayout() + ForgotPasswordConfirmationPath;
            return View(pathView);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            var pathView = GetLayout() + ResetPasswordPath;
            loginViewModel.Code = code;
            return View(pathView, loginViewModel);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(LoginViewModel model)
        {
            var pathView = GetLayout() + ResetPasswordPath;
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            UserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };
            UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            //user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
            //var res = UserManager.Update(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            else
            {
                LoginViewModel loginViewModel = new LoginViewModel();
                ViewBag.Message = "Reset password has been failed!";
                return View(pathView, loginViewModel);
            }
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            LoginViewModel loginViewModel = new LoginViewModel();
            var pathView = GetLayout() + ResetPasswordConfirmPath;
            return View(pathView, loginViewModel);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
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
            //var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            //if (result == SignInStatus.Failure)
            //{
            string email = myInfo["email"];
            string first_name = myInfo["first_name"];
            string last_name = myInfo["last_name"];
            string gender = myInfo["gender"];

            var user = new ApplicationUser { UserName = cId + email, Email = email, CompanyId = cId };

            //var userLogin = db.AspNetUsers.Where(u => u.Email == email && u.CompanyId == cId).FirstOrDefault();
            var userLogin = unitWork.Repository<AspNetUser>().Get(u => u.Email == email && u.CompanyId == cId);
            if (userLogin != null)
            {
                await SignInManager.PasswordSignInAsync(email, XsrfKeyPass, false, shouldLockout: false);
                return RedirectToAction("Index", "Home");
            }
            UserManager.UserValidator = new CustomUserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };
            var resultLogin = await UserManager.CreateAsync(user, XsrfKeyPass);

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
                unitWork.Repository<User>().Add(userModel);
                await unitWork.SaveChangesAsync();
                //db.Users.Add(userModel);
                //await db.SaveChangesAsync();

                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                return RedirectToAction("Index", "Home");
                //}
                //var infoNew = await AuthenticationManager.GetExternalLoginInfoAsync();
                //resultLogin = await UserManager.AddLoginAsync(user.Id, infoNew.Login);
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /Account/LogOff
        [HttpGet]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["Cart"] = null;
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
            //var lstUserAddress = db.GetUserAddresses.Where(u => u.Uid == idUser && u.AddressType == ((int)AddressType.ShippingAddress).ToString()).ToList();
            var lstUserAddress = unitWork.Repository<UserAddress>()
                .GetAll(u => u.Uid == idUser && u.AddressType == ((int)AddressType.ShippingAddress).ToString()).ToList();

            var model = Mapper.Map<List<UserAddress>, List<AddressDTO>>(lstUserAddress);
            var pathView = GetLayout() + ListShippingAddressPath;
            return View(pathView, model);
        }

        public ActionResult ListBillingAddress()
        {
            var idUser = GetIdUserCurrent();
            //var lstUserAddress = db.GetUserAddresses.Where(u => u.Uid == idUser && u.AddressType == ((int)AddressType.BillingAddress).ToString()).ToList();
            var lstUserAddress = unitWork.Repository<UserAddress>()
                .GetAll(u => u.Uid == idUser && u.AddressType == ((int)AddressType.BillingAddress).ToString()).ToList();

            var model = Mapper.Map<List<UserAddress>, List<AddressDTO>>(lstUserAddress);
            var pathView = GetLayout() + ListBillingAddressPath;
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
            //User user = db.GetUsers.Where(u => u.Id == id).FirstOrDefault();
            User user = unitWork.Repository<User>().Get(u => u.Id == id);
            var model = Mapper.Map<User, UserDTO>(user);
            var pathView = GetLayout() + InforCustomerPath;
            return View(pathView, model);
        }
        [HttpPost]
        public ActionResult InforCustomer(UserDTO userDTO)
        {
            int id = GetIdUserCurrent();
            if (id == -1)
            {
                return RedirectToAction("Login");
            }

            var pathView = GetLayout() + InforCustomerPath;
            if (ModelState.IsValid)
            {
                User model = Mapper.Map<UserDTO, User>(userDTO);
                model.UpdatedAt = DateTime.Now;
                model.UserType = "N";
                model.Status = "1";
                unitWork.Repository<User>().Update(model);
                unitWork.SaveChanges();
                //db.Entry(model).State = EntityState.Modified;
                //db.Entry(model).Property("PaymentId").IsModified = false;
                //db.Entry(model).Property("Password").IsModified = false;
                //db.Entry(model).Property("FacebookId").IsModified = false;
                //db.Entry(model).Property("CreatedAt").IsModified = false;
                //db.Entry(model).Property("Avatar").IsModified = false;
                //db.Entry(model).Property("Email").IsModified = false;
                //db.SaveChanges();
                ViewBag.Message = SBSMessages.MessageUpdateInformationSuccess;
            }

            return View(pathView, userDTO);
        }
        public ActionResult OrderHistory(int? page, string productName, string dateFrom, string dateTo, string orderStatus)
        {
            int id = GetIdUserCurrent();
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (id == -1)
            {
                return RedirectToAction("Login");
            }
            //var order = db.GetOrders.Where(u => u.UId == id).ToList();
            var order = unitWork.Repository<Order>().GetAll(u => u.UId == id).ToList();
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;
            ViewBag.ProductName = productName;
            ViewBag.OrderStatus = GetListOrderStatus(orderStatus);
            ViewBag.OrderStatusId = orderStatus;
            if (!string.IsNullOrEmpty(productName))
            {
                productName = productName.Trim();
                //var newOrder = (from od in db.GetOrderDetails
                //                join o in db.GetOrders on od.OrderId equals o.OrderId
                //                where od.ProductName.Contains(productName)
                //                where o.UId == id
                //                select o).ToList();
                var newOrder = (from od in unitWork.Repository<OrderDetail>().GetAll(m=>m.CompanyId == cId)
                                join o in unitWork.Repository<Order>().GetAll(m=>m.CompanyId == cId) on od.OrderId equals o.OrderId
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
                order = order.Where(o => o.OrderStatus == int.Parse(orderStatus)).ToList();
            }

            var model = Mapper.Map<List<Order>, List<OrderDTO>>(order);
            foreach (var item in model)
            {
                //item.PaymentName = db.Payments.Any(p => p.PaymentId == item.PaymentId) ? db.Payments.Find(item.PaymentId).Name : "";
                //item.OrderDetails = db.GetOrderDetails.Where(o => o.OrderId == item.OrderId).ToList();
                item.PaymentName = unitWork.Repository<Payment>().Any(p => p.PaymentId == item.PaymentId) 
                    ? unitWork.Repository<Payment>().Find(item.PaymentId).Name : "";
                item.OrderDetails = unitWork.Repository<OrderDetail>().GetAll(o => o.OrderId == item.OrderId).ToList();
                item.DeliveryStatus = this.GetStatusByCode(item.DeliveryStatus);
            }

            var pathView = GetLayout() + OrderHistoryPath;
            ViewBag.OrderStatus = GetListOrderStatus();
            return View(pathView, model.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Adds the cart when login.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        private void AddCartWhenLogin(int id, int count)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                cart.LstOrder = new List<Models.Base.Order>();
            }

            List<Product> products = SBSCommon.Instance.GetProducts();
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();

            bool successAdd = false;
            foreach (var item in cart.LstOrder)
            {
                if (item.Product.Product_ID == id)
                {
                    item.Count = item.Count + count;
                    cart.Total = cart.Total + count * item.Product.Selling_Price;
                    successAdd = true;
                    break;
                }
            }

            if (!successAdd && product != null)
            {
                Models.Base.Order orderItem = new Models.Base.Order();
                orderItem.Product = product;
                orderItem.Count = count;
                cart.Total = cart.Total + count * orderItem.Product.Selling_Price;
                cart.LstOrder.Add(orderItem);
            }
            cart.Tax = SBSCommon.Instance.GetTaxOfProduct();
            Session["Cart"] = cart;
        }

        /// <summary>
        /// Adds the cart.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        private void AddCart(int id, int count)
        {
            //Get session Cart
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            else
            {
                cart.LstOrder = new List<Models.Base.Order>();
            }

            List<Product> products = SBSCommon.Instance.GetProducts();
            var product = products.Where(m => m.Product_ID == id).FirstOrDefault();

            bool successAdd = false;
            foreach (var item in cart.LstOrder)
            {
                if (item.Product.Product_ID == id)
                {
                    item.Count = item.Count + count;
                    cart.Total = cart.Total + count * item.Product.Selling_Price;
                    successAdd = true;
                    break;
                }
            }

            if (!successAdd && product != null)
            {
                Models.Base.Order orderItem = new Models.Base.Order();
                orderItem.Product = product;
                orderItem.Count = count;
                cart.Total = cart.Total + count * orderItem.Product.Selling_Price;
                cart.LstOrder.Add(orderItem);
            }
            cart.Tax = SBSCommon.Instance.GetTaxOfProduct();
            Session["Cart"] = cart;

            //If exist login save to cart of user
            var userID = GetIdUserCurrent();
            if (userID != -1)
            {
                //var cartOfDatabase = db.Carts.Where(m => m.UserId == userID && m.ProID == id).FirstOrDefault();
                var cartOfDatabase = unitWork.Repository<Cart>().Get(m => m.UserId == userID && m.ProID == id);
                if (cartOfDatabase != null && cartOfDatabase.Quantity > 1)
                {
                    cartOfDatabase.Quantity = cartOfDatabase.Quantity + count;
                    unitWork.Repository<Cart>().Update(cartOfDatabase);                    
                    //db.SaveChanges();
                }
                else
                {
                    cartOfDatabase = new Models.Cart();
                    cartOfDatabase.CompanyId = cId;
                    cartOfDatabase.ProID = id;
                    cartOfDatabase.Quantity = count;
                    cartOfDatabase.UserId = userID;
                    cartOfDatabase.IsPreOrder = product.Allowable_PreOrder;
                    cartOfDatabase.PreOrderNotice = product.Delivery_Noted;
                    //db.Carts.Add(cartOfDatabase);
                    //db.SaveChanges();
                    unitWork.Repository<Cart>().Add(cartOfDatabase);

                }
                unitWork.SaveChanges();
            }
        }

        public ActionResult DuplicateOrder(string id)
        {
            //var lstOrderDetails = db.GetOrderDetails.Where(m => m.OrderId == id);
            var lstOrderDetails = unitWork.Repository<OrderDetail>().GetAll(m => m.OrderId == id).ToList();
            foreach (var item in lstOrderDetails)
            {
                AddCart((int)item.ProId, item.Quantity);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
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
            //UserAddress userAddress = db.GetUserAddresses.Where(a => a.Id == id).FirstOrDefault();
            UserAddress userAddress = unitWork.Repository<UserAddress>().Get(a => a.Id == id);
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
            AddressDTO userAddress = new AddressDTO();
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, userAddress);
        }

        [HttpGet]
        public ActionResult AddBillingAddress()
        {
            var pathView = GetLayout() + AddBillinggAddressPath;
            AddressDTO userAddress = new AddressDTO();
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
            AddressDTO userAddress = new AddressDTO();
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddShippingAddress(AddressDTO userAddress)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<AddressDTO, UserAddress>(userAddress);
                    model.Uid = GetIdUserCurrent();

                    AddNewAddress(userAddress, model);
                    TempData["Message"] = SBSMessages.MessageAddShippingAddressSuccess;
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
        /// Return screen add shipping address page checkout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddBillingAddressCheckOut()
        {
            var pathView = GetLayout() + AddBillingAddressCheckOutPath;
            AddressDTO userAddress = new AddressDTO();
            ViewBag.Country = GetListCountry("Singapore");
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBillingAddress(AddressDTO userAddress)
        {
            if (ModelState.IsValid)
            {
                var model = Mapper.Map<AddressDTO, UserAddress>(userAddress);
                model.Uid = GetIdUserCurrent();
                AddNewAddress(userAddress, model);
                TempData["Message"] = SBSMessages.MessageAddBillingAddressSuccess;
                return RedirectToAction("ListBillingAddress");
            }
            ViewBag.Country = GetListCountry(userAddress.Country);
            var pathView = GetLayout() + AddBillinggAddressPath;
            return View(pathView, userAddress);
        }
        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddShippingAddressCheckOut(AddressDTO userAddress)
        {
            if (ModelState.IsValid)
            {
                var model = Mapper.Map<AddressDTO, UserAddress>(userAddress);
                model.Uid = GetIdUserCurrent();
                AddNewAddress(userAddress, model);
                TempData["Message"] = SBSMessages.MessageAddShippingAddressSuccess;
                return RedirectToAction("CheckoutAddress", "Orders");
            }
            ViewBag.Country = GetListCountry(userAddress.Country);
            var pathView = GetLayout() + AddShippingAddressPath;
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Function add shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddBillingAddressCheckOut(AddressDTO userAddress)
        {
            if (ModelState.IsValid)
            {
                var model = Mapper.Map<AddressDTO, UserAddress>(userAddress);
                model.Uid = GetIdUserCurrent();
                AddNewAddress(userAddress, model);
                TempData["Message"] = SBSMessages.MessageAddShippingAddressSuccess;
                return RedirectToAction("CheckoutAddress", "Orders");
            }
            ViewBag.Country = GetListCountry(userAddress.Country);
            var pathView = GetLayout() + AddShippingAddressPath;
            return View(pathView, userAddress);
        }

        /// <summary>
        /// Return screen add shipping address page checkout
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditShippingAddress(int id)
        {
            //var shippingAddress = db.UserAddresses.Find(id);
            var shippingAddress = unitWork.Repository<UserAddress>().Find(id);
            var model = Mapper.Map<UserAddress, AddressDTO>(shippingAddress);

            var pathView = GetLayout() + EditShippingAddressPath;
            ViewBag.Country = GetListCountry(shippingAddress.Country);
            return View(pathView, model);
        }
        /// <summary>
        /// Function edit shipping address to database screen checkout
        /// </summary>
        /// <param name="userAddress"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditShippingAddress(AddressDTO userAddress)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = Mapper.Map<AddressDTO, UserAddress>(userAddress);
                    model.UpdatedAt = DateTime.Now;
                    unitWork.Repository<UserAddress>().Update(model);
                    unitWork.SaveChanges();
                    //db.Entry(model).State = EntityState.Modified;
                    //db.Entry(model).Property("CreatedAt").IsModified = false;
                    //db.Entry(model).Property("Uid").IsModified = false;
                    //db.SaveChanges();
                    if (model.AddressType == ((int)AddressType.ShippingAddress).ToString())
                    {
                        TempData["Message"] = SBSMessages.MessageUpdateShippingAddressSuccess;
                        return RedirectToAction("ListShippingAddress");
                    }
                    if (model.AddressType == ((int)AddressType.BillingAddress).ToString())
                    {
                        TempData["Message"] = SBSMessages.MessageUpdateBillingAddressSuccess;
                        return RedirectToAction("ListBillingAddress");
                    }

                }
                ViewBag.Country = GetListCountry(userAddress.Country);
                AddressDTO addressDTO = new AddressDTO();
                //Redirect to page shipping adrress
                if (userAddress.AddressType == ((int)AddressType.ShippingAddress).ToString())
                {
                    var pathViewEditShippingAddressPath = GetLayout() + EditShippingAddressPath;
                    return View(pathViewEditShippingAddressPath, addressDTO);
                }
                //Redirect to page billing adrress
                var pathViewEditBillingAddressPath = GetLayout() + EditBillingAddressPath;
                return View(pathViewEditBillingAddressPath, addressDTO);

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
        /// Function choose address shipping and address billing for customer
        /// </summary>
        /// <param name="shippingAddressId">shipping address id</param>
        /// <param name="billingAddressId">billing address id</param>
        /// <param name="isBillingAddress">is billing address</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChooseAddressShipping(int shippingAddressId, int billingAddressId, bool isBillingAddress)
        {
            Models.Base.Cart cart = new Models.Base.Cart();
            if (Session["Cart"] != null)
            {
                cart = (Models.Base.Cart)Session["Cart"];
            }
            if (isBillingAddress)
            {
                cart.billingAddressId = billingAddressId;
            }
            cart.shippingAddressId = shippingAddressId;

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CheckoutPayment"),
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Delete address by id address
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public async Task<ActionResult> AddressDelete(int addressId)
        {
            //var customer = db.UserAddresses;

            //find address (ensure that it belongs to the current customer)
            //var address = customer.FirstOrDefault(a => a.Id == addressId);
            var address = unitWork.Repository<UserAddress>().Get(a => a.Id == addressId);

            if (address != null)
            {
                //db.UserAddresses.Remove(address);

                //await db.SaveChangesAsync();
                unitWork.Repository<UserAddress>().Delete(address);
                await unitWork.SaveChangesAsync();
            }

            if (address.AddressType == ((int)AddressType.ShippingAddress).ToString())
            {
                TempData["Message"] = SBSMessages.MessageDeletedShippingAddressSuccess;
                //redirect to the shipping address list page
                return Json(new
                {
                    redirect = Url.RouteUrl("ListShippingAddress"),
                });
            }
            TempData["Message"] = SBSMessages.MessageDeleteBillingAddressSuccess;
            //redirect to the billing address list page
            return Json(new
            {
                redirect = Url.RouteUrl("ListBillingAddress"),
            });

        }
        /// <summary>
        /// Change avartar customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeAvatar()
        {
            var pathView = GetLayout() + ChangeAvatarPath;
            //var user = db.Users.Find(GetIdUserCurrent());
            var user = unitWork.Repository<User>().Find(GetIdUserCurrent());

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
        public ActionResult CreditPoint()
        {
            try
            {
                //var point = db.GetUsers.Where(u => u.Id == uid).Select(u => u.CreditPoint).FirstOrDefault();
                var point = unitWork.Repository<User>().Find(GetIdUserCurrent()).CreditPoint;
                ViewBag.Points = point;
            }
            catch (Exception e)
            {
                ViewBag.Points = 0;
            }
            var pathView = GetLayout() + CreditPointPath;
            return View(pathView);
        }

        /// <summary>
        /// Revove avartar customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult RemoveAvatar()
        {
            var pathView = GetLayout() + ChangeAvatarPath;
            //var user = db.Users.Find(GetIdUserCurrent());
            var user = unitWork.Repository<User>().Find(GetIdUserCurrent());
            user.Avatar = null;
            user.UpdatedAt = DateTime.Now;
            unitWork.Repository<User>().Update(user);
            unitWork.SaveChanges();
            //db.Entry(user).State = EntityState.Modified;
            //db.SaveChanges();

            return Json(new
            {
                redirect = Url.RouteUrl("ChangeAvatar"),
            }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Update avartart customer
        /// </summary>
        /// <param name="file">Image customer</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAvatar(HttpPostedFileBase file)
        {
            //var user = db.Users.Find(GetIdUserCurrent());
            var user = unitWork.Repository<User>().Find(GetIdUserCurrent());

            if (file != null && file.ContentLength > 0)
                try
                {
                    string uniqueNameAvatar = cId + "_" + CommonUtil.GetNameUnique() + "_" + file.FileName;
                    string path = Path.Combine(Server.MapPath(SBSConstants.LINK_UPLOAD_AVATAR),
                                               Path.GetFileName(uniqueNameAvatar));
                    file.SaveAs(path);

                    // remove old avatar
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        CommonUtil.DeleteFile(Server.MapPath(user.Avatar));
                    }
                    user.Avatar = SBSConstants.LINK_UPLOAD_AVATAR + uniqueNameAvatar;
                    user.UpdatedAt = DateTime.Now;
                    unitWork.Repository<User>().Update(user);
                    unitWork.SaveChanges();
                    //db.Entry(user).State = EntityState.Modified;
                    //db.SaveChanges();
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


        #endregion

        #region Page ProductReview
        [Authorize]
        public ActionResult ProductReviews()
        {
            //var id = GetIdUserCurrent();
            //var productReviews = db.ProductReviews.Where(p => p.UId == id).ToList();
            //var model = Mapper.Map<List<ProductReview>, List<ProductReviewDTO>>(productReviews);
            var pathView = GetLayout() + ProductReviewPath;
            //return View(pathView, model);
            return View(pathView);
        }
        #endregion

        #region Validation
        public JsonResult CheckExistsEmail(string email)
        {
            if (email == null)
            {
                return Json(new { status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            //User user = db.GetUsers.Where(u => u.Email == email).FirstOrDefault();
            User user = unitWork.Repository<User>().Get(u => u.Email == email);
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
                items.Add(new SelectListItem { Text = "Singapore", Value = "Singapore", Selected = false });
            }
            if (selected == "Thailand")
            {

                items.Add(new SelectListItem { Text = "Thailand", Value = "Thailand", Selected = true }); ;
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

            items.Add(new SelectListItem { Text = "All", Value = null, Selected = false });

            if (status == OrderStatus.Delivering.ToString())
            {

                items.Add(new SelectListItem { Text = OrderStatus.Delivering.ToString(), Value = ((int)OrderStatus.Delivering).ToString(), Selected = true });
            }
            else
            {
                items.Add(new SelectListItem { Text = OrderStatus.Delivering.ToString(), Value = ((int)OrderStatus.Delivering).ToString(), Selected = false });
            }
            if (status == OrderStatus.Processing.ToString())
            {

                items.Add(new SelectListItem { Text = OrderStatus.Processing.ToString(), Value = ((int)OrderStatus.Processing).ToString(), Selected = true }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = OrderStatus.Processing.ToString(), Value = ((int)OrderStatus.Processing).ToString(), Selected = false });
            }
            if (status == OrderStatus.Completed.ToString())
            {

                items.Add(new SelectListItem { Text = OrderStatus.Completed.ToString(), Value = ((int)OrderStatus.Completed).ToString(), Selected = true }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = OrderStatus.Completed.ToString(), Value = ((int)OrderStatus.Completed).ToString(), Selected = false });
            }
            if (status == OrderStatus.Cancelled.ToString())
            {

                items.Add(new SelectListItem { Text = OrderStatus.Cancelled.ToString(), Value = ((int)OrderStatus.Cancelled).ToString(), Selected = true }); ;
            }
            else
            {
                items.Add(new SelectListItem { Text = OrderStatus.Cancelled.ToString(), Value = ((int)OrderStatus.Cancelled).ToString(), Selected = false });
            }
            return items;
        }
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        private const string XsrfKeyPass = "WannacryCrypto@2017";

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

        private void AddNewAddress(AddressDTO userAddress, UserAddress model)
        {
            //var userAdd = db.UserAddresses.Find(model.Uid);
            var userAdd = unitWork.Repository<UserAddress>().Find(model.Uid);
            if (userAdd == null)
            {
                model.DefaultType = true;
            }

            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.AddressType = userAddress.AddressType;
            //db.UserAddresses.Add(model);
            //db.SaveChanges();
            unitWork.Repository<UserAddress>().Add(model);
            unitWork.SaveChanges();
        }
    }
}