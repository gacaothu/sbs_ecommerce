using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Models;
using System.Data.Entity.Validation;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Globalization;
using SBS_Ecommerce.Framework.Utilities;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using SBS_Ecommerce.Models.DTOs;
using System.Security.Claims;
using Microsoft.Owin.Security;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Models.Extension;
using System.Data.Entity;
using Facebook;
using System.Dynamic;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MailChimp.Net.Models;
using AutoMapper;

namespace SBS_Ecommerce.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        //List<Models.Base.Theme> themes = new List<Models.Base.Theme>();
        private const string PathConfigTheme = "~/Content/theme.xml";
        private const string PathBlock = "~/Content/block.xml";
        private const string PathPage = "~/Content/page.xml";

        private const string PathOrder = "~/Views/Admin/Orders.cshtml";
        private const string PathPartialOrder = "~/Views/Admin/_PartialOrder.cshtml";
        private const string PathPartialOrderDetail = "~/Views/Admin/OrderDetail.cshtml";
        private const string PathProfile = "~/Views/Admin/Profile.cshtml";
        private const string PathDeliveryScheduler = "~/Views/Admin/DeliveryScheduler.cshtml";
        private const string PathPartialDeliveryScheduler = "~/Views/Admin/_PartialDeliverySchedulerDetail.cshtml";
        private const string PathPartialDeliveryCompany = "~/Views/Admin/_PartialDeliveryCompanyDetail.cshtml";
        private const string PathPartialSEODetail = "~/Views/Admin/_PartialSEODetail.cshtml";

        Helper helper = new Helper();

        private List<UnitOfMass> unitOfMass = new List<UnitOfMass>()
                {
                    new UnitOfMass() { Value = "g", Name="Gram" },
                    new UnitOfMass() { Value = "kg", Name="Kilogram" },
                    new UnitOfMass() { Value = "lbs", Name="Pound" }
                };

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginDTO adminLoginDTO)
        {
            for (int i = 0; i < 5; i++)
            {
                var passEncrypt = EncryptUtil.Encrypt(adminLoginDTO.PassWord);
                string url = SBSConstants.LINK_APILOGIN + "?u=" + adminLoginDTO.UserName + "&p=" + passEncrypt;
                var result = RequestUtil.SendRequest(url);
                var json = JsonConvert.DeserializeObject<LoginAdminDTO>(result);
                var account = json.Items;
                if (json.Return_Code == 1)
                {
                    var ident = new ClaimsIdentity(new[] {
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier,account.Email),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                    // optionally you could add roles if any
                    new Claim(ClaimTypes.Role, "Admin"),
                },
                    DefaultAuthenticationTypes.ApplicationCookie);
                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = false }, ident);
                    Session[account.Email] = account;
                    return RedirectToAction("ThemeManager");
                }
            }

            ViewBag.MessageError = "User name or Password is incorrect.";
            return View(adminLoginDTO);
        }
        [HttpGet]
        public ActionResult Logout()
        {
            //Removing Session
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Admin");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            string url = SBSConstants.LINK_APIFORGOTPASSOWRD + email;
            var result = RequestUtil.SendRequest(url);
            var json = JsonConvert.DeserializeObject<ForgotPasswordDTO>(result);
            if (json != null && json.Return_Code == 1)
            {
                ViewBag.MessageSuccess = "Please check your email to reset your password.";
            }
            else
            {
                ViewBag.MessageError = json.Msg;
            }
            return View();
        }
        /// <summary>
        /// Theme manage
        /// </summary>
        /// <returns>Views</returns>
        public ActionResult ThemeManager()
        {
            //var themes = db.Themes.Where(m => m.CompanyId == cId).ToList();
            var themes = db.GetThemes.ToList();
            ViewBag.Themes = themes;
            ViewBag.Title = "Theme Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ChangeLayout(List<int> lstID)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Layout> lstLayoutNew = new List<Layout>();
            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~/Views/Theme/") + cId.ToString() + "/" + theme.Name + "/layout.xml");

            //foreach (var itemID in lstID)
            //{
            //    foreach (var itemLayout in lstLayout)
            //    {
            //        if (itemID == itemLayout.ID)
            //        {
            //            lstLayoutNew.Add(new Layout { ID = itemLayout.ID, Name = itemLayout.Name, Content = itemLayout.Content, Path = itemLayout.Path, Active = itemLayout.Active, CanEdit = itemLayout.CanEdit, Type = itemLayout.Type });
            //        }
            //    }
            //}

            //helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml", lstLayoutNew);

            // new source
            var lstLayout = db.GetConfigLayouts.ToList();
            for (int i = 0; i < lstID.Count; i++)
            {
                int lid = lstID[i];
                var layout = lstLayout.FirstOrDefault(m => m.Id == lid);
                layout.Position = i + 1;
            }
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayoutManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Layout> lstLayout = new List<Layout>();
            //lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
            ////Session["RenderLayout"] = lstLayout;
            //ViewBag.RenderLayout = lstLayout;

            // new source
            var theme = db.GetThemes.FirstOrDefault(m => m.Active);
            var lstLayout = db.GetConfigLayouts.Where(m => m.Active).OrderBy(m => m.Position).ToList();
            ViewBag.RenderLayout = lstLayout;

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            //Slider slider = new Slider();
            //slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configslider.xml");
            List<ConfigSlider> sliders = db.GetConfigSliders.ToList();
            ViewBag.RenderSlider = sliders;
            ViewBag.Title = "Layout Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ActiveBlock(int id)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            //List<Layout> lstLayoutNew = new List<Layout>();
            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
            //lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = true;
            //helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);

            var layout = db.GetConfigLayouts.FirstOrDefault(m => m.Id == id);
            if (layout != null)
            {
                layout.Active = true;
                layout.UpdatedAt = DateTime.Now;
                db.SaveChanges();
            }
            
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeactiveBlock(int id)
        {
            try
            {
                //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
                //List<Layout> lstLayoutNew = new List<Layout>();
                //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
                //lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = false;
                //helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);

                // new source
                //var theme = db.GetThemes.FirstOrDefault(m => m.Active);
                var layout = db.GetConfigLayouts.FirstOrDefault(m => m.Id == id);
                if (layout != null)
                {
                    layout.Active = false;
                    layout.UpdatedAt = DateTime.Now;
                    db.SaveChanges();
                }             

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
            // return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHTML(int id)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Layout> lstLayoutNew = new List<Layout>();
            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
            //var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();

            // new source
            var layout = db.GetConfigLayouts.FirstOrDefault(m => m.Id == id);
            return Json(new { Title = layout?.Name, Content = layout?.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddHTML(string content, string title)
        {
            try
            {
                //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
                //List<Layout> lstLayoutNew = new List<Layout>();
                //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
                //Layout layout = new Layout();
                //layout.ID = lstLayout.Max(m => m.ID) + 1;
                //if (string.IsNullOrEmpty(title))
                //{
                //    layout.Name = "HTML/JavaScript";
                //}
                //else
                //{
                //    layout.Name = title;
                //}

                //layout.Path = "\\Widget\\_PartialHTML.cshtml";
                //layout.Content = content;
                //layout.Active = true;
                //layout.CanEdit = true;
                //lstLayout.Add(layout);
                //helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);

                // new source
                ConfigLayout layout = new ConfigLayout()
                {
                    CompanyId = cId,
                    Path = "\\Widget\\_PartialHTML.cshtml",
                    Position = db.GetConfigLayouts.Count() + 1,
                    Name = !string.IsNullOrEmpty(title) ? title : "HTML/JavaScript",
                    Content = content,
                    Active = true,
                    CanEdit = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.ConfigLayouts.Add(layout);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditHTML(string content, string title, int id)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            //List<Layout> lstLayoutNew = new List<Layout>();
            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");
            //var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();
            //layout.Content = content;
            //layout.Name = title;
            //helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);

            // new source
            var layout = db.GetConfigLayouts.FirstOrDefault(m => m.Id == id);
            if (layout != null)
            {
                layout.Name = title;
                layout.Content = content;
                layout.UpdatedAt = DateTime.Now;
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview(string id)
        {
            string[] lstID = id.Split('_');

            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            //List<Layout> lstLayoutNew = new List<Layout>();
            //Session["Layout"] = theme.Path + "/Index.cshtml";

            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");

            //foreach (var itemID in lstID)
            //{
            //    foreach (var itemLayout in lstLayout)
            //    {
            //        if (itemID.ToString().Trim() == itemLayout.ID.ToString().Trim())
            //        {
            //            lstLayoutNew.Add(new Layout { ID = itemLayout.ID, Name = itemLayout.Name, Content = itemLayout.Content, Path = itemLayout.Path, Active = itemLayout.Active, CanEdit = itemLayout.CanEdit, Type = itemLayout.Type });
            //        }
            //    }
            //}

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");

            // new source
            var theme = db.GetThemes.FirstOrDefault(m => m.Active);
            var lstLayoutPreview = new List<ConfigLayout>();
            var lstLayout = db.GetConfigLayouts.ToList();
            for (int i = 0; i < lstID.Length; i++)
            {
                int lid = int.Parse(lstID[i]);
                var layout = lstLayout.FirstOrDefault(m => m.Id == lid);
                layout.Position = i + 1;
                lstLayoutPreview.Add(layout);
            }

            //Get category from API
            //ViewBag.LstCategory = helper.GetCategory();
            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            ViewBag.LstBlog = db.GetBlogs.ToList();

            ViewBag.RenderMenu = db.GetConfigMenus.ToList();
            //ViewBag.RenderLayout = lstLayoutNew;
            ViewBag.RenderLayout = lstLayoutPreview.OrderBy(m => m.Position).ToList();

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            return View(theme.PathView + "/Index.cshtml");
        }

        public ActionResult PreViewMenu(string id)
        {
            string[] lstID = id.Split('_');
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenuNew = new List<Menu>();
            //Session["Layout"] = theme.Path + "/Index.cshtml";

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");

            //foreach (var itemID in lstID)
            //{
            //    foreach (var itemLayout in lstMenu)
            //    {
            //        if (itemID.ToString().Trim() == itemLayout.ID.ToString().Trim())
            //        {
            //            lstMenuNew.Add(new Menu { ID = itemLayout.ID, Name = itemLayout.Name, Href = itemLayout.Href, LstChildMenu = itemLayout.LstChildMenu });
            //        }
            //    }
            //}

            //List<Layout> lstLayout = new List<Layout>();
            //lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/layout.xml");

            // new source
            var theme = db.GetThemes.FirstOrDefault(m => m.Active);
            var lstMenuPreview = new List<ConfigMenu>();
            var lstMenu = db.GetConfigMenus.ToList();
            for (int i = 0; i < lstID.Length; i++)
            {
                int mid = int.Parse(lstID[i]);
                var menu = lstMenu.FirstOrDefault(m => m.MenuId == mid);
                menu.Position = i + 1;
                lstMenuPreview.Add(menu);
            }            
            
            var lstLayout = db.GetConfigLayouts.Where(m=>m.Active).ToList();

            //Get category from API
            //ViewBag.LstCategory = helper.GetCategory();
            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            //ViewBag.RenderMenu = lstMenuNew;
            ViewBag.RenderMenu = lstMenuPreview.OrderBy(m => m.Position).ToList();
            ViewBag.LstBlog = db.GetBlogs.ToList();
            //ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();
            ViewBag.RenderLayout = lstLayout;

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            return View(theme.PathView + "/Index.cshtml");
        }

        [HttpPost]
        public ActionResult AddGadgate()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeActive(int id)
        {
            try
            {
                var theme = db.Themes.Where(m => m.ID == id).FirstOrDefault();


                //Set all theme to false
                db.Themes.Where(m => m.CompanyId == cId).ToList().ForEach(m => m.Active = false);

                //Set theme to true
                theme.Active = true;

                //Save XML
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Upload()
        {
            try
            {
                //HttpPostedFileBase file = Request.Files[0]; //Uploaded file
                //int fileSize = file.ContentLength;
                //string fileName = file.FileName;
                //string mimeType = file.ContentType;
                //Stream fileContent = file.InputStream;

                //var description = Request.Form["Description"];

                //string extractPath = Server.MapPath("~/") + "/Views/Theme/" + cId + "/ExtraTheme/";
                //if (!Directory.Exists(extractPath))
                //{
                //    Directory.CreateDirectory(extractPath);
                //}
                ////To save file, use SaveAs method
                //string pathSave = Server.MapPath("~/") + "/Views/Theme/" + cId + "/ExtraTheme/" + fileName;
                //file.SaveAs(pathSave); //File will be saved in application root

                ////Extra zip file
                //string zipPath = pathSave;

                //ZipFile.ExtractToDirectory(zipPath, extractPath);

                ////Copy folder to Content
                //helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Content", Server.MapPath("~/Content/Theme/") + cId + "/" + fileName.Replace(".zip", ""), true);

                ////Copy folder to Views
                //helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Views", Server.MapPath("~/Views/Theme/") + cId + "/" + fileName.Replace(".zip", ""), true);

                ////Save to database

                //Models.Theme theme = new Models.Theme();
                //theme.Name = fileName.Replace(".zip", "");
                //theme.Path = "~/Views/Theme/" + cId + "/" + fileName.Replace(".zip", "");
                //theme.Description = description;
                //db.Themes.Add(theme);
                //db.SaveChanges();

                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        public ActionResult GetInforTheme(int id)
        {
            var theme = db.Themes.Where(m => m.ID == id).FirstOrDefault();
            if (theme.Description != null)
                return Json(theme.Description, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTheme(int id)
        {
            try
            {
                var theme = db.Themes.Where(m => m.ID == id).FirstOrDefault();
                string extractDelete = Server.MapPath("~/") + "/Views/Theme/" + cId.ToString() + "/ExtraTheme/" + theme.Name;

                //Delete folder on extra
                Directory.Delete(extractDelete, true);

                //Delete file .zip
                System.IO.File.Delete(extractDelete + ".zip");

                //Delete on view
                Directory.Delete(Server.MapPath("~/") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name, true);

                //Delete on content
                Directory.Delete(Server.MapPath("~/") + "/Content/Theme/" + cId.ToString() + "/" + theme.Name, true);

                //If theme on active then set themedefault active
                if (theme.Active)
                {
                    db.Themes.Where(m => m.Name == "Default" && m.CompanyId == cId).FirstOrDefault().Active = true;
                }

                //Remove theme in database
                db.Themes.Remove(theme);
                db.SaveChanges();

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveConfigSlider()
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            //Slider slider = new Slider();
            //slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configslider.xml");

            ////Uploaded file
            //for (int i = 0; i < Request.Files.Count; i++)
            //{
            //    var file = Request.Files[i];
            //    int fileSize = file.ContentLength;
            //    string fileName = file.FileName;
            //    string mimeType = file.ContentType;
            //    System.IO.Stream fileContent = file.InputStream;
            //    var id = Request.Files.Keys[i];

            //    //Path content of theme
            //    var pathContentofTheme = Server.MapPath("~/") + "/Content/Theme/" + cId.ToString() + "/" + theme.Name;

            //    //Check exist folder img
            //    if (!Directory.Exists(pathContentofTheme + "/img"))
            //    {
            //        Directory.CreateDirectory(pathContentofTheme + "/img");
            //    }

            //    //Check exist folder slider
            //    if (!Directory.Exists(pathContentofTheme + "/img/slider"))
            //    {
            //        Directory.CreateDirectory(pathContentofTheme + "/img/slider");
            //    }


            //    //To save file, use SaveAs method
            //    var random = RandomString();
            //    string pathSave = pathContentofTheme + "/img/slider/" + random + fileName;

            //    file.SaveAs(pathSave); //File will be saved in application root

            //    //Remove file if exist
            //    var picture = slider.LstPicture.Where(m => m.ID == id).FirstOrDefault();
            //    if (System.IO.File.Exists(Server.MapPath(picture.Path)))
            //    {
            //        System.IO.File.Delete(Server.MapPath(picture.Path));
            //    }

            //    picture.Path = "/Content/Theme/" + cId.ToString() + "/" + theme.Name + "/img/slider/" + random + fileName;
            //}

            ////Remove file and path
            //var idfromForm = System.Web.HttpContext.Current.Request.Form["id"];
            //if (idfromForm != null)
            //{
            //    var lstId = idfromForm.Split(',');
            //    foreach (var item in lstId)
            //    {
            //        //Remove file if exist
            //        var picture = slider.LstPicture.Where(m => m.ID == item.ToString()).FirstOrDefault();
            //        if (System.IO.File.Exists(Server.MapPath(picture.Path)))
            //        {
            //            System.IO.File.Delete(Server.MapPath(picture.Path));
            //        }
            //        picture.Path = string.Empty;
            //    }
            //}

            ////Change config xml
            //helper.SerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configslider.xml", slider);


            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private string RandomString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public ActionResult MenuManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> menu = new List<Menu>();
            //menu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");
            var menu = db.GetConfigMenus.OrderBy(m=>m.Position).ToList();
            ViewBag.Title = "Menu Manager";
            ViewBag.LstMenu = menu;
            ViewBag.Pages = db.GetPages.ToList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMenu(string name, string url)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();
            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");

            ////Create menu
            //Menu menu = new Menu();
            //menu.ID = lstMenu.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            //menu.Name = name;
            //menu.Href = url;

            ////Save to xml configmenu
            //lstMenu.Add(menu);
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            ConfigMenu menu = new ConfigMenu()
            {
                CompanyId = cId,
                Name = name,
                Href = url,
                Position = db.GetConfigMenus.Count() + 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            db.ConfigMenus.Add(menu);
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMenu(int id, string name, string url)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");

            //var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            //menu.Name = name;
            //menu.Href = url;

            ////Save to xml configmenu
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            var menu = db.GetConfigMenus.FirstOrDefault(m => m.MenuId == id);
            if (menu != null)
            {
                menu.Name = name;
                menu.Href = url;
                menu.UpdatedAt = DateTime.Now;
                db.SaveChanges();
            }

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMenu(int id)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");
            //var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            //lstMenu.Remove(menu);

            ////Save to xml configmenu
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            var menu = db.GetConfigMenus.FirstOrDefault(m => m.MenuId == id);
            if (menu != null)
            {
                db.Entry(menu).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddChildMenu(int id, string name, string url)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");
            //var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();

            //ChildMenu childMenu = new ChildMenu();
            //if (menu.LstChildMenu != null && menu.LstChildMenu.Count > 0)
            //{
            //    childMenu.ID = menu.LstChildMenu.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            //}
            //else
            //{
            //    childMenu.ID = 1;
            //}

            //childMenu.Name = name;
            //childMenu.Href = url;
            //menu.LstChildMenu.Add(childMenu);

            ////Save to xml configmenu
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            var menu = db.GetConfigMenus.FirstOrDefault(m => m.MenuId == id);
            if (menu != null)
            {
                ConfigChildMenu childmenu = new ConfigChildMenu()
                {
                    CompanyId = cId,
                    MenuId = menu.MenuId,
                    Name = name,
                    Href = url,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.ConfigChildMenus.Add(childmenu);
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditChildMenu(int parentID, int childrenID, string name, string url)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");
            //var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            ////Get childmenu
            //var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            //childMenu.Name = name;
            //childMenu.Href = url;

            ////Save to xml configmenu
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            var childmenu = db.GetConfigChildMenus.FirstOrDefault(m => m.Id == childrenID && m.MenuId == parentID);
            if (childmenu != null)
            {
                childmenu.Name = name;
                childmenu.Href = url;
                childmenu.UpdatedAt = DateTime.Now;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteChildMenu(int parentID, int childrenID)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");
            //var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            ////Get childmenu
            //var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            //menu.LstChildMenu.Remove(childMenu);

            ////Save to xml configmenu
            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            // new source
            var childmenu = db.GetConfigChildMenus.FirstOrDefault(m => m.Id == childrenID && m.MenuId == parentID);
            if (childmenu != null)
            {
                db.Entry(childmenu).State = EntityState.Deleted;
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMenu(List<int> lstID)
        {
            //var theme = db.Themes.Where(m => m.Active && m.CompanyId == cId).FirstOrDefault();

            //List<Menu> lstMenu = new List<Menu>();
            //lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml");

            //List<Menu> lstMenuNew = new List<Menu>();
            //foreach (var itemID in lstID)
            //{
            //    foreach (var itemLayout in lstMenu)
            //    {
            //        if (itemID == itemLayout.ID)
            //        {
            //            lstMenuNew.Add(new Models.Base.Menu { ID = itemLayout.ID, Name = itemLayout.Name, Href = itemLayout.Href, LstChildMenu = itemLayout.LstChildMenu });
            //        }
            //    }
            //}

            //helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenuNew);

            // new source
            for (int i = 0; i < lstID.Count; i++)
            {
                var id = lstID[i];
                var menu = db.GetConfigMenus.FirstOrDefault(m => m.MenuId == id);
                menu.Position = i + 1;
            }
            db.SaveChanges();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }
            var lstBlock = db.Blocks.Where(m => m.CompanyId == cId).ToList();
            return View(lstBlock);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlock(string title, string content)
        {
            Models.Block block = new Models.Block();
            block.Name = title;
            block.Content = content;
            block.CompanyId = cId;
            db.Blocks.Add(block);

            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentBlock(int id)
        {
            var block = db.Blocks.Where(m => m.ID == id && m.CompanyId == cId).FirstOrDefault();
            return Json(new { Title = block.Name, Content = block.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlock(int id, string title, string content)
        {
            var block = db.Blocks.Where(m => m.CompanyId == cId && m.ID == id).FirstOrDefault();

            block.Name = title;
            block.Content = content;

            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete block
        /// </summary>
        /// <param name="id">Block ID</param>
        /// <returns>Status</returns>
        [HttpPost]
        public ActionResult DeleteBlock(int id)
        {
            var block = db.Blocks.Where(m => m.CompanyId == cId && m.ID == id).FirstOrDefault();

            db.Blocks.Remove(block);
            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Page Manager
        /// </summary>
        /// <returns>View</returns>
        public ActionResult PageManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            var lstPage = db.Pages.Where(m => m.CompanyId == cId).ToList();
            return View(lstPage);
        }

        public ActionResult CheckDuplicateNamePage(string name,int? id)
        {
            if (id!=null)
            {
                var page=db.Pages.Find(id);
                if (page.Name==name)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var check = db.Pages.Where(m => m.Name.ToUpper() == name.ToUpper() && m.CompanyId == cId).FirstOrDefault();

                    return Json(check != null, JsonRequestBehavior.AllowGet);
                }
            }
            var result = db.Pages.Where(m => m.Name.ToUpper() == name.ToUpper() && m.CompanyId == cId).FirstOrDefault();

            return Json(result != null, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPage(string title, string content, bool usingLayout)
        {
            Models.Page page = new Models.Page();
            page.Name = title;
            page.Content = content;
            page.CompanyId = cId;
            page.UsingLayout = usingLayout;
            db.Pages.Add(page);

            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPage(int id, string title, string content, bool usingLayout)
        {
            var page = db.Pages.Where(m => m.ID == id && m.CompanyId == cId).FirstOrDefault();

            page.Name = title;
            page.Content = content;
            page.UsingLayout = usingLayout;
            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentPage(int id)
        {
            var page = db.Pages.Where(m => m.CompanyId == cId && m.ID == id).FirstOrDefault();
            return Json(new { Title = page.Name, Content = page.Content, UsingLayout = page.UsingLayout }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePage(int id)
        {
            var page = db.Pages.Where(m => m.ID == id && m.CompanyId == cId).FirstOrDefault();

            db.Pages.Remove(page);
            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlogManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            var lstBlog = db.GetBlogs.ToList();
            return View(lstBlog);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlog(string title, string content, string path)
        {
            try
            {
                Blog blog = new Blog();
                blog.Title = title;
                blog.BlogContent = content;
                blog.CreatedAt = DateTime.Now;
                blog.UpdatedAt = DateTime.Now;
                blog.Status = "1";
                blog.Thumb = path;
                db.Blogs.Add(blog);
                db.SaveChanges();
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

            // var x = System.Web.HttpContext.Current.Request.Form["id"];
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBlog(int id)
        {
            var blog = db.GetBlogs.Where(m => m.BlogId == id).FirstOrDefault();
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadImageThumbnail()
        {
            var path = "/Content/img/blog/";
            //Uploaded file
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;

                //Path content of theme
                var pathContentofTheme = Server.MapPath("~/Content/img/blog/");

                //To save file, use SaveAs method
                var random = RandomString();
                string pathSave = pathContentofTheme + random + fileName;
                path = path + random + fileName;
                file.SaveAs(pathSave); //File will be saved in application root
            }
            //Return status
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentBlog(int id)
        {
            var blog = db.GetBlogs.Where(m => m.BlogId == id).FirstOrDefault();
            var thumb = "";
            if (!string.IsNullOrEmpty(blog.Thumb))
            {
                thumb = Url.Content("~" + blog.Thumb);
            }
            else
            {
                thumb = blog.Thumb;
            }
            return Json(new { Title = blog.Title, Content = blog.BlogContent, Thumb = thumb }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlog(int id, string title, string content, string thumb)
        {
            var blog = db.GetBlogs.Where(m => m.BlogId == id).FirstOrDefault();
            blog.Title = title;
            blog.BlogContent = content;
            if (thumb != "nochange")
            {
                blog.Thumb = thumb;
            }

            blog.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarketingManager()
        {
            List<Marketing> lstMarketing = db.GetMarketings.ToList();
            return View(lstMarketing);
        }

        public async Task<ActionResult> PushToMailChimp()
        {
            // Instantiate new manager
            // Get apiKey
            var apiKey = db.ConfigMailChimps.FirstOrDefault();
            if (apiKey != null)
            {

                IMailChimpManager mailChimpManager = new MailChimpManager(apiKey.ApiKey);
                var mailChimpListCollection = await mailChimpManager.Lists.GetAllAsync().ConfigureAwait(false);
                var listId = mailChimpListCollection.FirstOrDefault().Id;
                var x = "\"";
                // Use the Status property if updating an existing member
                var lstUser = db.Users.Where(m => (m.PushMailChimp == null || m.PushMailChimp == false) && m.CompanyId == cId).ToList();

                foreach (var item in lstUser)
                {
                    //Update status mailchimp
                    var user = db.Users.Where(m => m.Id == item.Id).FirstOrDefault();
                    try
                    {
                        var member = new Member { EmailAddress = $"{item.Email}", StatusIfNew = Status.Subscribed };
                        member.MergeFields.Add("FNAME", item.FirstName);
                        member.MergeFields.Add("LNAME", item.LastName);
                        await mailChimpManager.Members.AddOrUpdateAsync(listId, member);
                    }
                    catch
                    {

                    }

                    user.PushMailChimp = true;
                    db.SaveChanges();


                }

                return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AccountManager()
        {
            var lstUser = db.Users.Where(m => m.CompanyId == cId).ToList();
            return View(lstUser);
        }


        public ActionResult SendMailManager(int id)
        {
            ViewBag.IDMarketing = id;
            List<ScheduleEmail> lstScheduleEmail = db.GetScheduleEmails.Where(m => m.MarketingID == id).ToList();
            return View(lstScheduleEmail);
        }

        /// <summary>
        /// Create campaign
        /// </summary>
        /// <param name="name">Campaign Name</param>
        /// <param name="content">Campaign Content</param>
        /// <returns>Return status</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CreateCampaign(string name, string content)
        {
            //Create new object marketing
            Marketing marketing = new Marketing();
            marketing.NameCampain = name;
            marketing.Content = content;

            //Add to database
            db.Marketings.Add(marketing);

            //Save change
            db.SaveChanges();

            //Return status change
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Get information of campaign
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <returns>Return object campaign</returns>
        public ActionResult GetCampaign(int id)
        {
            //Get campaign from db with id
            var marketing = db.GetMarketings.Where(m => m.Id == id).FirstOrDefault();

            //Return object campaign
            return Json(new { NameCampain = marketing.NameCampain, Content = marketing.Content }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edit information of campaign
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <param name="name">Campaign Name</param>
        /// <param name="content">Campaign Content</param>
        /// <returns>Return status</returns>
        [ValidateInput(false)]
        public ActionResult EditCampaign(int id, string name, string content)
        {
            //Get campaign from db with id
            var campaign = db.GetMarketings.Where(m => m.Id == id).FirstOrDefault();
            if (campaign != null)
            {
                campaign.NameCampain = name;
                campaign.Content = content;
                db.SaveChanges();
            }

            //Return status update
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete Campaign
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <returns>Return status</returns>
        public ActionResult DeleteCampaign(int id)
        {
            var campaign = db.GetMarketings.Where(m => m.Id == id).FirstOrDefault();
            if (campaign != null)
            {
                db.Marketings.Remove(campaign);
                db.SaveChanges();
            }

            //Return status update
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmailSystem()
        {
            var email = "";
            foreach (var item in db.Users)
            {
                if (!string.IsNullOrEmpty(item.Email))
                    email = email + " " + item.Email;
            }

            return Json(email, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Change status campaign
        /// </summary>
        /// <param name="id">Campaign ID</param>
        /// <param name="active">Active</param>
        /// <returns>Return status</returns>
        public ActionResult ChangeStatusCampaign(int id)
        {
            var campaign = db.GetMarketings.Where(m => m.Id == id).FirstOrDefault();
            if (campaign != null)
            {
                if (campaign.Status != null && (bool)campaign.Status)
                    campaign.Status = false;
                else
                    campaign.Status = true;
                db.SaveChanges();
            }

            //Return status update
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchedual(int id)
        {
            var chEmail = db.GetScheduleEmails.Where(m => m.ID == id).FirstOrDefault();
            return Json(new { Email = chEmail.Email, Subject = chEmail.Subject, Schedule = chEmail.Schedule }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSchedual(int id)
        {
            var chEmail = db.GetScheduleEmails.Where(m => m.ID == id).FirstOrDefault();
            if (chEmail != null)
            {
                db.ScheduleEmails.Remove(chEmail);
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SendMail(int id, string time, List<string> lstEmail, string subject)
        {
            try
            {
                var emailmarketing = db.GetMarketings.Where(m => m.Id == id).FirstOrDefault();
                DateTime datetime = new DateTime();
                if (!string.IsNullOrEmpty(time))
                {
                    datetime = DateTime.Parse(time, new CultureInfo("en-US", true));
                }
                else
                {
                    datetime = DateTime.Now;
                }

                //Create Schedual
                ScheduleEmail schEmail = new ScheduleEmail();
                schEmail.Email = String.Join(" ", lstEmail).Trim();
                schEmail.MarketingID = id;
                schEmail.Schedule = datetime;
                schEmail.Subject = subject;
                schEmail.Status = false;
                db.ScheduleEmails.Add(schEmail);
                db.SaveChanges();

                //DateTime datetime = new DateTime();
                var emailMessage = emailmarketing.Content;
                await SendEmail(subject, emailMessage, datetime, lstEmail, schEmail.ID);
            }
            catch
            {

            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveShedualEmail(int id, string time, List<string> lstEmail, string subject)
        {
            var scEmail = db.GetScheduleEmails.Where(m => m.ID == id).FirstOrDefault();
            if (!(bool)scEmail.Status)
            {
                DateTime datetime = new DateTime();
                if (!string.IsNullOrEmpty(time))
                {
                    datetime = DateTime.Parse(time, new CultureInfo("en-US", true));
                }
                else
                {
                    datetime = DateTime.Now;
                }

                scEmail.Email = String.Join(" ", lstEmail).Trim();
                scEmail.Schedule = datetime;
                scEmail.Subject = subject;
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private async Task SendEmail(string emailSubject, string emailMessage, DateTime time, List<string> lstEmail, int id)
        {
            var message = new MailMessage();
            foreach (var item in lstEmail)
            {
                message.To.Add(item);
            }

            message.Subject = emailSubject;
            message.Body = emailMessage;
            message.IsBodyHtml = true;
            await SendAwait(message, id);
        }

        private async Task SendAwait(MailMessage message, int id)
        {
            Task t = Task.Run(() =>
            {
                var schEmail = db.GetScheduleEmails.Where(m => m.ID == id).FirstOrDefault();
                if (DateTime.Now > schEmail.Schedule)
                {
                    //To do
                    EmailUtil emailUT = new EmailUtil();
                    emailUT.SendListEmail(message);
                    schEmail.Status = true;
                    db.SaveChanges();
                }
                else
                {
                    while (DateTime.Now < schEmail.Schedule)
                    {
                        schEmail = db.GetScheduleEmails.Where(m => m.ID == id).FirstOrDefault();
                        if (schEmail == null || schEmail.Schedule == null)
                        {
                            return;
                        }
                        System.Threading.Thread.Sleep(2000);
                    }
                    //To do
                    EmailUtil emailUT = new EmailUtil();
                    emailUT.SendListEmail(message);
                    schEmail.Status = true;
                    db.SaveChanges();
                }

                // System.Threading.Thread.Sleep((int)milisecon);

            });
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult ChattingManager()
        {
            return View(db.GetConfigChattings.FirstOrDefault());
        }

        public ActionResult SaveConfigMailChimp(string apiKey)
        {
            var configMailChimp = db.ConfigMailChimps.Where(m => m.CompanyId == cId).FirstOrDefault();
            if (configMailChimp == null && !string.IsNullOrEmpty(apiKey))
            {
                ConfigMailChimp cfMailChimp = new ConfigMailChimp();
                cfMailChimp.CompanyId = cId;
                cfMailChimp.ApiKey = apiKey;
                db.ConfigMailChimps.Add(cfMailChimp);
                db.SaveChanges();
            }
            else
            {
                if (!string.IsNullOrEmpty(apiKey))
                {
                    configMailChimp.ApiKey = apiKey;
                    db.SaveChanges();
                }
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveConfigChatting(string pageID)
        {
            if (db.GetConfigChattings.Count() == 0 && !string.IsNullOrEmpty(pageID))
            {
                ConfigChatting cfChatting = new ConfigChatting();
                cfChatting.PageID = pageID;
                cfChatting.PathPage = pageID;
                db.ConfigChattings.Add(cfChatting);

                db.SaveChanges();
            }
            else
            {
                if (!string.IsNullOrEmpty(pageID))
                {
                    var cfChatting = db.GetConfigChattings.FirstOrDefault();
                    cfChatting.PageID = pageID;
                    db.SaveChanges();
                }
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlogComment(int id)
        {
            var lstBlogComment = db.BlogComments.Where(m => m.CompanyId == cId && m.BlogId == id).ToList();
            return View(lstBlogComment);
        }

        [HttpPost]
        public ActionResult DeleteBlogComment(int id)
        {
            var blogComment = db.BlogComments.Where(m => m.Id == id).FirstOrDefault();
            if (blogComment != null)
            {
                db.BlogComments.Remove(blogComment);
                db.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        #region Configuration
        public ActionResult ShippingFee()
        {
            try
            {
                ViewBag.WeightBaseds = db.GetWeightBaseds.ToList();
                ViewBag.LocalPickup = db.GetLocalPickups.FirstOrDefault();
                ViewBag.Countries = CountryUtil.Instance.GetCountries();
                ViewBag.DeliveryCompanies = db.GetDeliveryCompanies.ToList();
                ViewBag.WeightBasedEnable = db.GetConfigShippings.Where(m => m.Name.Contains("Weight Based")).FirstOrDefault();
                ViewBag.LocalPickupEnable = db.GetConfigShippings.Where(m => m.Name.Contains("Local Pickup")).FirstOrDefault();
                ViewBag.UnitOfMass = unitOfMass;
            }
            catch
            {

            }
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateWeightBased(WeightBased model)
        {
            bool check = true;
            var errMsg = "";
            try
            {
                model.CompanyId = cId;
                model.CreatedAt = DateTime.Now;
                db.WeightBaseds.Add(model);
                db.SaveChanges();
            }
            catch
            {
                check = false;
                errMsg = "Error occurred while creating Weight based item...";
            }
            if (check)
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { Status = SBSConstants.Failed, Message = errMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Configuration()
        {
            var configPaypal = db.GetConfigPaypals.FirstOrDefault();
            var configPaypalDTO = AutoMapper.Mapper.Map<ConfigPaypal, ConfigPaypalDTO>(configPaypal);
            var configEmail = db.GetEmailAccounts.FirstOrDefault();
            var configEmailDTO = AutoMapper.Mapper.Map<EmailAccount, EmailAccountDTO>(configEmail);
            var configMailChimp = db.ConfigMailChimps.Where(m => m.CompanyId == cId).FirstOrDefault();

            ViewBag.ConfigChatting = db.GetConfigChattings.FirstOrDefault();
            ViewBag.ConfigPaypalDTO = configPaypalDTO;
            ViewBag.ConfigEmail = configEmailDTO;
            ViewBag.ConfigMailChimp = configMailChimp;
            return View();
        }

        [HttpPost]
        public ActionResult ConfigPaypal(string Id, string Mode, int? ConnectionTimeout, string ClientId, string ClientSecret)
        {
            ConfigPaypalDTO configPaypalDTO = new ConfigPaypalDTO();


            configPaypalDTO.Mode = Mode;
            configPaypalDTO.ConnectionTimeout = ConnectionTimeout;
            configPaypalDTO.ClientId = ClientId;
            configPaypalDTO.ClientSecret = ClientSecret;

            if (!string.IsNullOrEmpty(Id))
            {
                configPaypalDTO.Id = int.Parse(Id);
                try
                {
                    ViewBag.Message = " Configuration has been updated successfully.";
                    var configPaypal = AutoMapper.Mapper.Map<ConfigPaypalDTO, ConfigPaypal>(configPaypalDTO);
                    db.Entry(configPaypal).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(true);
                }
                catch (Exception)
                {
                    ViewBag.Message = " Configuration has been updated failed.";
                    return Json(false);
                }
            }
            else
            {
                var configPaypal = AutoMapper.Mapper.Map<ConfigPaypalDTO, ConfigPaypal>(configPaypalDTO);
                db.Entry(configPaypal).State = System.Data.Entity.EntityState.Added;
                db.SaveChanges();
                return Json(true);
            }


        }

        [HttpPost]
        public ActionResult ConfigEmail(string Id, string Email, string DisplayName, string Host, int Port, string Username, string Password, bool EnableSsl, bool UseDefaultCredentials)
        {
            EmailAccountDTO emailAccountDTO = new EmailAccountDTO();
            emailAccountDTO.Host = Host;
            emailAccountDTO.DisplayName = DisplayName;
            emailAccountDTO.Email = Email;
            emailAccountDTO.EnableSsl = EnableSsl;
            emailAccountDTO.Password = Password;
            emailAccountDTO.Port = Port;
            emailAccountDTO.UseDefaultCredentials = UseDefaultCredentials;
            emailAccountDTO.Username = Username;

            if (!string.IsNullOrEmpty(Id))
            {
                emailAccountDTO.Id = int.Parse(Id);
                try
                {
                    ViewBag.Message = " Configuration has been updated successfully.";
                    var configEmail = AutoMapper.Mapper.Map<EmailAccountDTO, EmailAccount>(emailAccountDTO);
                    db.Entry(configEmail).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return Json(true);
                }
                catch (Exception)
                {
                    ViewBag.Message = " Configuration has been updated failed.";
                    return Json(false);
                }
            }
            else
            {
                try
                {
                    ViewBag.Message = " Configuration has been updated successfully.";
                    var configEmail = AutoMapper.Mapper.Map<EmailAccountDTO, EmailAccount>(emailAccountDTO);
                    db.Entry(configEmail).State = System.Data.Entity.EntityState.Added;
                    db.SaveChanges();
                    return Json(true);
                }
                catch (Exception)
                {
                    ViewBag.Message = " Configuration has been updated failed.";
                    return Json(false);
                }
            }

        }
        #endregion

        /// <summary>
        /// Get Orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders(int kind, string startDate, string endDate, string textSearch)
        {
            try
            {
                ViewBag.Kind = kind;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.TextSearch = textSearch;
                switch (kind)
                {
                    case (int)OrderStatus.Pending:
                        ViewBag.Data = GetOrders(OrderStatus.Pending, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Pending.ToString();
                        break;
                    case (int)OrderStatus.Processing:
                        ViewBag.Data = GetOrders(OrderStatus.Processing, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Processing.ToString();
                        break;
                    case (int)OrderStatus.Completed:
                        ViewBag.Data = GetOrders(OrderStatus.Completed, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Completed.ToString();
                        break;
                    case (int)OrderStatus.Cancelled:
                        ViewBag.Data = GetOrders(OrderStatus.Completed, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Completed.ToString();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) { }
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

            List<OrderDetail> details = new List<OrderDetail>();
            try
            {
                var data = db.GetOrders.Where(m => m.OrderId == id).Include(m => m.OrderDetails).Include(m => m.User).
                    Include(m => m.Payment).FirstOrDefault();
                ViewBag.BillingAddress = db.GetUserAddresses.Where(m => m.Id == data.BillingAddressId).FirstOrDefault();
                ViewBag.ShippingAddress = db.GetUserAddresses.Where(m => m.Id == data.ShippingAddressId).FirstOrDefault();
                ViewBag.Order = data;
            }
            catch (Exception e)
            {
            }
            return View(PathPartialOrderDetail);
        }

        /// <summary>
        /// Updates the shipping status.
        /// </summary>
        /// <param name="id">The order identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateStatus(string id)
        {
            bool flag = false;
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            try
            {
                var order = db.GetOrders.FirstOrDefault(c => c.OrderId == id);

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
                        await StockOut(order.OrderId);
                        break;
                    default:
                        flag = false;
                        break;
                }

                if (flag)
                {
                    order.UpdatedAt = DateTime.Now;
                    var entry = db.Entry(order);
                    entry.Property(m => m.OrderStatus).IsModified = true;
                    entry.Property(m => m.ShippingStatus).IsModified = true;
                    entry.Property(m => m.UpdatedAt).IsModified = true;
                    db.SaveChanges();
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Call api update quanlity on store 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<int> StockOut(string orderId)
        {
            ListStockOutDTO lstStockOutDTO = new ListStockOutDTO();
            var lstOrderDetail = db.GetOrderDetails.Where(o => o.OrderId == orderId).ToList();
            StockOutDTO stockOutDTO = null;
            OutputStockOut outputStockOut = null;
            foreach (var item in lstOrderDetail)
            {
                stockOutDTO = new StockOutDTO();
                stockOutDTO.Product_ID = item.ProId;
                stockOutDTO.Quantity = item.Quantity;
                lstStockOutDTO.stks.Add(stockOutDTO);
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SBSConstants.LINK_API_STOCKOUT);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string value = JsonConvert.SerializeObject(lstStockOutDTO);
                StringContent content = new StringContent(JsonConvert.SerializeObject(lstStockOutDTO), Encoding.UTF8, "application/json");
                //StringContent content = new StringContent(JsonConvert.SerializeObject(lstStockOutDTO));
                // HTTP POST
                HttpResponseMessage response = await client.PostAsync(client.BaseAddress, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    outputStockOut = JsonConvert.DeserializeObject<OutputStockOut>(data);
                }
            }
            return outputStockOut.Return_Code;
        }
        /// <summary>
        /// Delivery Company Management
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryCompany()
        {
            ViewBag.Data = db.GetDeliveryCompanies.ToList();
            ViewBag.Countries = CountryUtil.Instance.GetCountries();
            return View("~/Views/Admin/DeliveryCompanyManager.cshtml");
        }

        [HttpPost]
        public ActionResult InsertOrUpdateDeliveryCompany(DeliveryCompany model)
        {
            string message = "";
            try
            {
                if (model.Id == 0)
                {
                    model.CompanyId = cId;
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;

                    db.DeliveryCompanies.Add(model);
                }
                else
                {
                    model.UpdatedAt = DateTime.Now;
                    db.DeliveryCompanies.Attach(model);
                    var entry = db.Entry(model);
                    entry.Property(m => m.Address).IsModified = true;
                    entry.Property(m => m.City).IsModified = true;
                    entry.Property(m => m.District).IsModified = true;
                    entry.Property(m => m.CompanyName).IsModified = true;
                    entry.Property(m => m.Phone).IsModified = true;
                    entry.Property(m => m.Email).IsModified = true;
                    entry.Property(m => m.Country).IsModified = true;
                    entry.Property(m => m.Fax).IsModified = true;
                    entry.Property(m => m.UpdatedAt).IsModified = true;
                    entry.Property(m => m.Ward).IsModified = true;
                }

                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                message = "Error occurred while adding Delivery Company";
                return Json(new { Status = SBSConstants.Failed, Message = message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteDeliveryCompany(int id)
        {
            try
            {
                var entity = new DeliveryCompany() { Id = id };
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { Status = SBSConstants.Failed }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetDeliveryCompany(int id)
        {
            string viewStr = "";
            try
            {
                var dc = db.GetDeliveryCompanies.Where(m => m.Id == id).FirstOrDefault();
                ViewBag.Countries = CountryUtil.Instance.GetCountries();
                viewStr = PartialViewToString(this, PathPartialDeliveryCompany, dc);

                return Json(new { Status = SBSConstants.Success, Partial = viewStr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = SBSConstants.Failed, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DuplicateWeightBase(int id)
        {
            var msg = "";
            try
            {
                var item = db.GetWeightBaseds.Where(m => m.Id == id).FirstOrDefault();
                var clone = new WeightBased()
                {
                    CompanyId = cId,
                    Min = item.Min,
                    Max = item.Max,
                    Rate = item.Rate,
                    UnitOfMass = item.UnitOfMass,
                    Country = item.Country,
                    DeliveryCompany = item.DeliveryCompany,
                    CreatedAt = DateTime.Now
                };

                db.WeightBaseds.Add(clone);
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = msg }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetWeightBased(int id)
        {
            string result = "";
            try
            {
                ViewBag.DeliveryCompanies = db.GetDeliveryCompanies.ToList();
                ViewBag.Countries = CountryUtil.Instance.GetCountries();
                ViewBag.Model = db.GetWeightBaseds.Where(m => m.Id == id).FirstOrDefault();
                ViewBag.UnitOfMass = unitOfMass;

                result = PartialViewToString(this, "~/Views/Admin/_PartialWeightBasedDetail.cshtml", ViewBag.Model);
            }
            catch (Exception e)
            {
                result = e.Message;
            }
            return Json(new { Partial = result }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult UpdateWeightBased(WeightBased model)
        {
            var errMsg = "";
            try
            {
                model.UpdatedAt = DateTime.Now;
                db.WeightBaseds.Attach(model);
                var entry = db.Entry(model);
                entry.Property(e => e.Min).IsModified = true;
                entry.Property(e => e.Max).IsModified = true;
                entry.Property(e => e.Rate).IsModified = true;
                entry.Property(e => e.Country).IsModified = true;
                entry.Property(e => e.UnitOfMass).IsModified = true;
                entry.Property(e => e.DeliveryCompany).IsModified = true;
                entry.Property(e => e.UpdatedAt).IsModified = true;

                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = errMsg }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UpdateWeighBasedConfiguration()
        {
            string msg = "Success";
            try
            {
                var item = db.GetConfigShippings.Where(m => m.Name.Contains("Weight Based")).FirstOrDefault();
                if (item == null)
                {
                    item = new ConfigShipping()
                    {
                        Name = "Weight Based",
                        CompanyId = cId,
                        CreatedAt = DateTime.Now,
                        Description = "To calculate Shipping Fee via weight based.",
                        Status = true
                    };

                    db.ConfigShippings.Add(item);
                    db.SaveChanges();
                }
                else
                {
                    if (item.Status != null && item.Status.Value)
                        item.Status = false;
                    else
                        item.Status = true;

                    item.UpdatedAt = DateTime.Now;
                    db.ConfigShippings.Attach(item);

                    var entry = db.Entry(item);
                    entry.Property(e => e.Status).IsModified = true;
                    entry.Property(e => e.UpdatedAt).IsModified = true;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteWeightBased(int id)
        {
            try
            {
                WeightBased item = new WeightBased() { Id = id };
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = SBSConstants.Success, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateLocalPickupConfiguration()
        {
            string msg = "Success";
            try
            {
                var item = db.GetConfigShippings.Where(m => m.Name.Contains("Local Pickup")).FirstOrDefault();
                if (item == null)
                {
                    item = new ConfigShipping()
                    {
                        Name = "Local Pickup",
                        CompanyId = cId,
                        CreatedAt = DateTime.Now,
                        Description = "Free shipping fee if pick up items at local storage.",
                        Status = true
                    };

                    db.ConfigShippings.Add(item);
                    db.SaveChanges();
                }
                else
                {
                    if (item.Status != null && item.Status.Value)
                        item.Status = false;
                    else
                        item.Status = true;

                    item.UpdatedAt = DateTime.Now;
                    db.ConfigShippings.Attach(item);

                    var entry = db.Entry(item);
                    entry.Property(e => e.Status).IsModified = true;
                    entry.Property(e => e.UpdatedAt).IsModified = true;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            return Json(new { Message = msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateLocalPickupInfo(LocalPickup model)
        {
            string msg = "";
            try
            {
                var data = db.GetLocalPickups.ToList();
                if (data.IsNullOrEmpty())
                {
                    model.CompanyId = cId;
                    model.CreatedAt = DateTime.Now;
                    db.LocalPickups.Add(model);
                    db.SaveChanges();
                }
                else
                {
                    var item = db.GetLocalPickups.Where(m => m.Id == model.Id).FirstOrDefault();
                    item.Phone = model.Phone;
                    item.Ward = model.Ward;
                    item.District = model.District;
                    item.City = model.City;
                    item.Country = model.Country;
                    item.UpdatedAt = DateTime.Now;
                    db.LocalPickups.Attach(item);
                    var entry = db.Entry(item);
                    entry.Property(e => e.UpdatedAt).IsModified = true;
                    entry.Property(e => e.Address).IsModified = true;
                    entry.Property(e => e.Ward).IsModified = true;
                    entry.Property(e => e.District).IsModified = true;
                    entry.Property(e => e.City).IsModified = true;
                    entry.Property(e => e.Country).IsModified = true;
                    entry.Property(e => e.Phone).IsModified = true;

                    db.SaveChanges();
                }
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                msg = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = msg }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ViewProfile()
        {
            var email = AuthenticationManager.User.Claims.ToList()[0].Value;
            LoginAdmin profile = (LoginAdmin)Session[email];

            if (profile != null)
            {
                string url = string.Format(SBSConstants.GetProfile, profile.Profile_ID);
                var result = RequestUtil.SendRequest(url);
                var json = JsonConvert.DeserializeObject<ProfileDTO>(result);

                return View(Url.Content(PathProfile), json.Items);
            }

            return RedirectToAction("Login");
        }

        public ActionResult DeliveryScheduler()
        {
            ViewBag.Data = db.GetDeliverySchedulers.ToList();
            return View(Url.Content(PathDeliveryScheduler));
        }

        [HttpPost]
        public ActionResult InsertOrUpdateDeliveryScheduler(DeliveryScheduler model)
        {
            string errMsg = "";
            try
            {
                if (model.Id == 0)
                {
                    model.CompanyId = cId;
                    model.TimeSlot = string.IsNullOrEmpty(model.TimeSlot) ? model.FromHour + " - " + model.ToHour : model.TimeSlot;
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    db.DeliverySchedulers.Add(model);
                }
                else
                {
                    model.TimeSlot = string.IsNullOrEmpty(model.TimeSlot) ? model.FromHour + " - " + model.ToHour : model.TimeSlot;
                    model.UpdatedAt = DateTime.Now;
                    model.CompanyId = cId;
                    db.DeliverySchedulers.Attach(model);
                    var entry = db.Entry(model);
                    entry.Property(m => m.FromHour).IsModified = true;
                    entry.Property(m => m.ToHour).IsModified = true;
                    entry.Property(m => m.TimeSlot).IsModified = true;
                    entry.Property(m => m.Rate).IsModified = true;
                    entry.Property(m => m.IsActive).IsModified = true;
                    entry.Property(m => m.IsHoliday).IsModified = true;
                    entry.Property(m => m.IsWeekday).IsModified = true;
                    entry.Property(m => m.IsWeekend).IsModified = true;
                    entry.Property(m => m.PerSlot).IsModified = true;
                    entry.Property(m => m.UpdatedAt).IsModified = true;
                }
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = errMsg }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult DeleteDeliveryScheduler(int id)
        {
            string errMsg = "";
            try
            {
                DeliveryScheduler ds = new DeliveryScheduler()
                {
                    Id = id
                };
                db.Entry(ds).State = EntityState.Deleted;
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = errMsg }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetDeliveryScheduler(int id)
        {
            string viewStr = "";
            try
            {
                DeliveryScheduler ds = db.GetDeliverySchedulers.Where(m => m.Id == id).FirstOrDefault();
                viewStr = PartialViewToString(this, PathPartialDeliveryScheduler, ds);
                return Json(new { Status = SBSConstants.Success, Partial = viewStr }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                viewStr = e.Message;
                return Json(new { Status = SBSConstants.Failed, Message = viewStr }, JsonRequestBehavior.AllowGet);
            }

        }

        #region Configuration Holiday
        public ActionResult HolidayConfiguaration(int? id)
        {
            ViewBag.Year = GetListYear(id);
            List<ConfigHoliday> lstConfigHoliday = db.GetConfigHolidays.ToList();
            if (lstConfigHoliday == null)
            {
                lstConfigHoliday = new List<ConfigHoliday>();
            }
            if (id != null)
            {
                lstConfigHoliday = lstConfigHoliday.Where(c => c.HolidayDate != null && c.HolidayDate.Value.Year == id).ToList();
            }
            var lstHoliday = Mapper.Map<List<ConfigHoliday>, List<ConfigHolidayDTO>>(lstConfigHoliday);
            return View(lstHoliday);
        }
        [HttpGet]
        public ActionResult AddHoliday()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddHoliday(ConfigHolidayDTO configHolidayDTO)
        {
            var configHoliday = Mapper.Map<ConfigHolidayDTO, ConfigHoliday>(configHolidayDTO);
            configHoliday.CreateAt = DateTime.Now;
            db.Entry(configHoliday).State = EntityState.Added;
            db.SaveChanges();
            TempData["Message"] = SBSMessages.MessageAddHolidaySuccess;
            return RedirectToAction("HolidayConfiguaration");
        }
        [HttpGet]
        public ActionResult EditHoliday(int id)
        {
            var holiday = db.ConfigHolidays.Find(id);
            var configHoliday = Mapper.Map<ConfigHoliday, ConfigHolidayDTO>(holiday);
            return View(configHoliday);
        }
        [HttpPost]
        public ActionResult EditHoliday(ConfigHolidayDTO configHolidayDTO, bool? IsActive)
        {
            var configHoliday = Mapper.Map<ConfigHolidayDTO, ConfigHoliday>(configHolidayDTO);
            db.Entry(configHoliday).State = EntityState.Modified;
            db.Entry(configHoliday).Property("CreateAt").IsModified = false;
            db.SaveChanges();
            TempData["Message"] = SBSMessages.MessageUpdatedHolidaySuccess;
            return RedirectToAction("HolidayConfiguaration");
        }
        [HttpPost]
        public ActionResult DeleteHoliday(int id)
        {
            var holiday = db.GetConfigHolidays.Where(c => c.Id == id).FirstOrDefault();
            if (holiday == null)
            {
                TempData["MessageError"] = SBSMessages.MessageHolidaySuccessNotFound;
                return RedirectToAction("HolidayConfiguaration");
            }
            db.Entry(holiday).State = EntityState.Deleted;
            db.SaveChanges();
            TempData["Message"] = SBSMessages.MessageDeleteHolidaySuccess;
            return RedirectToAction("HolidayConfiguaration");
        }
        private List<SelectListItem> GetListYear(int? id)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            int year = DateTime.Now.Year + 5;
            if (id == null)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (year - i == DateTime.Now.Year)
                    {
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = false });
                    }

                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (year - i == id)
                    {
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = true });
                    }
                    else
                    {
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = false });
                    }

                }
            }

            return items;
        }
        #endregion

        private List<Models.Order> GetOrders(OrderStatus kind, string startDate, string endDate, string textSearch)
        {
            List<Models.Order> result = new List<Models.Order>();
            DateTime start;
            DateTime end;
            string dateFormat = "yyyy-MM-dd";
            try
            {
                // All fields contain values
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                {
                    start = DateTime.ParseExact(startDate, dateFormat, null);
                    end = DateTime.ParseExact(endDate, dateFormat, null);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt >= start && m.CreatedAt <= end &&
                        (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                        m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                        m.CountProduct.ToString().Contains(textSearch)
                        )
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Start date and End date have values
                else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(textSearch))
                {
                    start = DateTime.ParseExact(startDate, dateFormat, null);
                    end = DateTime.ParseExact(endDate, dateFormat, null).AddTicks(-1).AddDays(1);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt >= start && m.CreatedAt <= end
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Start date and Text search have values
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                {
                    start = DateTime.ParseExact(startDate, dateFormat, null);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt >= start &&
                        (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                        m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                        m.CountProduct.ToString().Contains(textSearch)
                        )
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // End date and Text search have values
                else if (string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                {
                    end = DateTime.ParseExact(endDate, dateFormat, null);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt <= end &&
                        (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                        m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                        m.CountProduct.ToString().Contains(textSearch)
                        )
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Only Start date has value
                else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(textSearch))
                {
                    start = DateTime.ParseExact(startDate, dateFormat, null);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt >= start
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Only End date has value
                else if (!string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(textSearch))
                {
                    end = DateTime.ParseExact(endDate, dateFormat, null);
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        m.CreatedAt <= end
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Only Text search has value
                else if (!string.IsNullOrEmpty(textSearch) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                {
                    result = db.GetOrders.Where(
                        m => m.OrderStatus == (int)kind &&
                        (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                        m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                        m.CountProduct.ToString().Contains(textSearch)
                        )
                    ).OrderBy(m => m.CreatedAt).ToList();
                }
                // Nothing
                else
                {
                    result = db.GetOrders.Where(m => m.OrderStatus == (int)kind).OrderBy(m => m.CreatedAt).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        public ActionResult SEO()
        {
            return View(db.GetSEOs.ToList());
        }

        public ActionResult GetSEO(int id)
        {
            try
            {
                return PartialView(PathPartialSEODetail, db.GetSEOs.FirstOrDefault(m => m.Id == id));
            }
            catch (Exception e)
            {
                return Json(new { Status = SBSConstants.Failed, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddOrUpdateSEO(SEO model)
        {
            try
            {
                model.CompanyId = cId;
                if (model.Id != 0)
                {
                    model.UpdatedAt = DateTime.Now;
                    db.SEOs.Attach(model);
                    db.Entry(model).State = EntityState.Modified;
                }
                else
                {
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    db.SEOs.Add(model);
                }
                db.SaveChanges();

                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = SBSConstants.Failed, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteSEO(int id)
        {
            try
            {
                SEO seo = new SEO() { Id = id };
                db.Entry(seo).State = EntityState.Deleted;
                db.SaveChanges();
                return Json(new { Status = SBSConstants.Success }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { Status = SBSConstants.Failed, Message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SharingManager()
        {
            string app_id = "156185678238922";
            string app_secret = "ac1b1dae834e0a0f6de7c8bbf5d9e80f";
            string scope = "publish_stream, publish_actions";

            var client = new FacebookClient();

            dynamic token = client.Get("oauth/access_token", new
            {
                client_id = app_id,
                client_secret = app_secret,
                grant_type = "client_credentials",
                scope = "publish_stream, publish_actions"
            });
            //client.AccessToken = "EAACEdEose0cBADVr0LInQjkXj1lPc9C0hYX5gXZBCNI5zFF3rFHvU44A35pKTSraqd2r998aLvzd7M2XKHe1M1bviKLt9rCLgxyHnjjZCLMqUZAHeiJacqcZBiNlOSQMuDnfEm8KcoaREZBlZBJxGJTUiDFOoZCJu4VWvJQO6iOK8HWiEmZAQ34ZBpKOJgNruGm8ZD";

            //dynamic parameters = new ExpandoObject();
            //parameters.title = "abcd";
            //parameters.message = "abcd";
            //parameters.link = "http://test.com/blog";

            //var result = client.Post("204566616622918" + "/feed", parameters);

            var identity = AuthenticationManager.GetExternalIdentity(DefaultAuthenticationTypes.ExternalCookie);
            var accessToken = identity.FindFirstValue("FacebookAccessToken");
            var fb = new FacebookClient(accessToken);
            dynamic parameters = new ExpandoObject();
            parameters.message = "Check out this funny article";
            parameters.link = "http://www.natiska.com/article.html";
            parameters.picture = "http://www.natiska.com/dav.png";
            parameters.name = "Article Title";
            parameters.caption = "Caption for the link";
            fb.Post("/117102342122260/feed", parameters);
            return View();
        }
    }
}