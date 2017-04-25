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
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SBS_Ecommerce.Models.DTOs;
using System.Security.Claims;
using Microsoft.Owin.Security;
using SBS_Ecommerce.Framework.Configurations;
using System.Net;
using SBS_Ecommerce.Models.Extension;

namespace SBS_Ecommerce.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        //List<Models.Base.Theme> themes = new List<Models.Base.Theme>();
        private const string pathConfigTheme = "~/Content/theme.xml";
        private const string pathBlock = "~/Content/block.xml";
        private const string pathPage = "~/Content/page.xml";

        private SBS_Entities db = new SBS_Entities();
        int cpID = SBSCommon.Instance.GetCompany().Company_ID;
        Helper helper = new Helper();

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginDTO adminLoginDTO)
        {
            var passEncrypt = EncryptUtil.Encrypt(adminLoginDTO.PassWord);
            string url = SBSConstants.LINK_APILOGIN + "?u=" + adminLoginDTO.UserName + "&p=" + passEncrypt;
            var result = RequestUtil.SendRequest(url);
            var json = JsonConvert.DeserializeObject<LoginAdminDTO>(result);
            var lstAdminLogin = json.Items;
            if (json.Return_Code == 1)
            {
                var ident = new ClaimsIdentity(
     new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier,lstAdminLogin.Email),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
              // optionally you could add roles if any
              new Claim(ClaimTypes.Role, "Admin"),
     },
     DefaultAuthenticationTypes.ApplicationCookie);
                HttpContext.GetOwinContext().Authentication.SignIn(
                   new AuthenticationProperties { IsPersistent = false }, ident);
                return RedirectToAction("ThemeManager");
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

        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Theme manage
        /// </summary>
        /// <returns>Views</returns>
        public ActionResult ThemeManager()
        {
            var themes = db.Themes.Where(m => m.CompanyId == cpID).ToList();
            ViewBag.Themes = themes;
            ViewBag.Title = "Theme Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ChangeLayout(List<int> lstID)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~/Views/Theme/") + cpID.ToString() + "/" + theme.Name + "/layout.xml");

            foreach (var itemID in lstID)
            {
                foreach (var itemLayout in lstLayout)
                {
                    if (itemID == itemLayout.ID)
                    {
                        lstLayoutNew.Add(new Layout { ID = itemLayout.ID, Name = itemLayout.Name, Content = itemLayout.Content, Path = itemLayout.Path, Active = itemLayout.Active, CanEdit = itemLayout.CanEdit, Type = itemLayout.Type });
                    }
                }
            }

            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml", lstLayoutNew);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayoutManager()
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Layout> lstLayout = new List<Layout>();
            lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
            //Session["RenderLayout"] = lstLayout;
            ViewBag.RenderLayout = lstLayout;

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            Slider slider = new Slider();
            slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configslider.xml");
            ViewBag.RenderSlider = slider;
            ViewBag.Title = "Layout Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ActiveBlock(int id)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
            lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = true;
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeactiveBlock(int id)
        {
            try
            {
                var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
                List<Layout> lstLayoutNew = new List<Layout>();
                var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
                lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = false;
                helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);
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
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
            var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();
            return Json(new { Title = layout.Name, Content = layout.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddHTML(string content, string title)
        {
            try
            {
                var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
                List<Layout> lstLayoutNew = new List<Layout>();
                var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
                Layout layout = new Layout();
                layout.ID = lstLayout.Max(m => m.ID) + 1;
                if (string.IsNullOrEmpty(title))
                {
                    layout.Name = "HTML/JavaScript";
                }
                else
                {
                    layout.Name = title;
                }

                layout.Path = "~\\Views\\Theme\\" + cpID.ToString() + "\\" + theme.Name + "\\Widget\\_PartialHTML.cshtml";
                layout.Content = content;
                layout.Active = true;
                layout.CanEdit = true;
                lstLayout.Add(layout);
                helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);
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
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");
            var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();
            layout.Content = content;
            layout.Name = title;
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml", lstLayout);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview(string id)
        {
            string[] lstID = id.Split('_');

            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            List<Layout> lstLayoutNew = new List<Layout>();
            Session["Layout"] = theme.Path + "/Index.cshtml";

            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");

            foreach (var itemID in lstID)
            {
                foreach (var itemLayout in lstLayout)
                {
                    if (itemID.ToString() == itemLayout.ID.ToString())
                    {
                        lstLayoutNew.Add(new Layout { ID = itemLayout.ID, Name = itemLayout.Name, Content = itemLayout.Content, Path = itemLayout.Path, Active = itemLayout.Active, CanEdit = itemLayout.CanEdit, Type = itemLayout.Type });
                    }
                }
            }

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");

            //Get category from API
            //ViewBag.LstCategory = helper.GetCategory();
            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            ViewBag.LstBlog = db.GetBlogs.ToList();

            ViewBag.RenderMenu = lstMenu.ToList();
            ViewBag.RenderLayout = lstLayoutNew;

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            return View(theme.Path + "/Index.cshtml");
        }

        public ActionResult PreViewMenu(string id)
        {
            string[] lstID = id.Split('_');
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenuNew = new List<Menu>();
            Session["Layout"] = theme.Path + "/Index.cshtml";

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");

            foreach (var itemID in lstID)
            {
                foreach (var itemLayout in lstMenu)
                {
                    if (itemID.ToString() == itemLayout.ID.ToString())
                    {
                        lstMenuNew.Add(new Menu { ID = itemLayout.ID, Name = itemLayout.Name, Href = itemLayout.Href, LstChildMenu = itemLayout.LstChildMenu });
                    }
                }
            }

            List<Layout> lstLayout = new List<Layout>();
            lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/layout.xml");

            //Get category from API
            //ViewBag.LstCategory = helper.GetCategory();
            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            ViewBag.RenderMenu = lstMenuNew;
            ViewBag.LstBlog = db.GetBlogs.ToList();
            ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();

            if (db.GetConfigChattings.FirstOrDefault() != null)
                ViewBag.PageID = db.GetConfigChattings.FirstOrDefault().PageID;

            return View(theme.Path + "/Index.cshtml");
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
                db.Themes.Where(m => m.CompanyId == cpID).ToList().ForEach(m => m.Active = false);

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
                HttpPostedFileBase file = Request.Files[0]; //Uploaded file
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                //To save file, use SaveAs method
                string pathSave = Server.MapPath("~/") + "/Views/Theme/" + cpID + "/ExtraTheme/" + fileName;
                file.SaveAs(pathSave); //File will be saved in application root

                //Extra zip file
                string zipPath = pathSave;
                string extractPath = Server.MapPath("~/") + "/Views/Theme/" + cpID + "/ExtraTheme/";
                ZipFile.ExtractToDirectory(zipPath, extractPath);

                //Copy folder to Content
                helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Content", Server.MapPath("~/Content/Theme/") + SBSCommon.Instance.GetCompany().Company_ID + "/" + fileName.Replace(".zip", ""), true);

                //Copy folder to Views
                helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Views", Server.MapPath("~/Views/Theme/") + SBSCommon.Instance.GetCompany().Company_ID + "/" + fileName.Replace(".zip", ""), true);

                //Save to database

                Models.Theme theme = new Models.Theme();
                theme.Name = fileName.Replace(".zip", "");
                theme.Path = "~/Views/Theme/" + cpID + "/" + fileName.Replace(".zip", "");
                db.Themes.Add(theme);
                db.SaveChanges();

                return Json(true);
            }
            catch
            {
                return Json(false);
            }
        }

        public ActionResult DeleteTheme(int id)
        {
            try
            {
                var theme = db.Themes.Where(m => m.ID == id).FirstOrDefault();
                string extractDelete = Server.MapPath("~/") + "/Views/Theme/" + cpID.ToString() + "/ExtraTheme/" + theme.Name;

                //Delete folder on extra
                Directory.Delete(extractDelete, true);

                //Delete file .zip
                System.IO.File.Delete(extractDelete + ".zip");

                //Delete on view
                Directory.Delete(Server.MapPath("~/") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name, true);

                //Delete on content
                Directory.Delete(Server.MapPath("~/") + "/Content/Theme/" + cpID.ToString() + "/" + theme.Name, true);

                //If theme on active then set themedefault active
                if (theme.Active)
                {
                    db.Themes.Where(m => m.Name == "Default" && m.CompanyId == cpID).FirstOrDefault().Active = true;
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
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            Slider slider = new Slider();
            slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configslider.xml");

            //Uploaded file
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;
                var id = Request.Files.Keys[i];

                //Path content of theme
                var pathContentofTheme = Server.MapPath("~/") + "/Content/Theme/" + cpID.ToString() + "/" + theme.Name;

                //Check exist folder img
                if (!Directory.Exists(pathContentofTheme + "/img"))
                {
                    Directory.CreateDirectory(pathContentofTheme + "/img");
                }

                //Check exist folder slider
                if (!Directory.Exists(pathContentofTheme + "/img/slider"))
                {
                    Directory.CreateDirectory(pathContentofTheme + "/img/slider");
                }


                //To save file, use SaveAs method
                var random = RandomString();
                string pathSave = pathContentofTheme + "/img/slider/" + random + fileName;

                file.SaveAs(pathSave); //File will be saved in application root

                //Remove file if exist
                var picture = slider.LstPicture.Where(m => m.ID == id).FirstOrDefault();
                if (System.IO.File.Exists(Server.MapPath(picture.Path)))
                {
                    System.IO.File.Delete(Server.MapPath(picture.Path));
                }

                picture.Path = "/Content/Theme/" + cpID.ToString() + "/" + theme.Name + "/img/slider/" + random + fileName;
            }

            //Remove file and path
            var idfromForm = System.Web.HttpContext.Current.Request.Form["id"];
            if (idfromForm != null)
            {
                var lstId = idfromForm.Split(',');
                foreach (var item in lstId)
                {
                    //Remove file if exist
                    var picture = slider.LstPicture.Where(m => m.ID == item.ToString()).FirstOrDefault();
                    if (System.IO.File.Exists(Server.MapPath(picture.Path)))
                    {
                        System.IO.File.Delete(Server.MapPath(picture.Path));
                    }
                    picture.Path = string.Empty;
                }
            }

            //Change config xml
            helper.SerializeSlider(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configslider.xml", slider);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private string RandomString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public ActionResult MenuManager()
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> menu = new List<Menu>();
            menu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");
            ViewBag.Title = "Menu Manager";
            ViewBag.LstMenu = menu;
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddMenu(string name, string url)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");

            //Create menu
            Menu menu = new Menu();
            menu.ID = lstMenu.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            menu.Name = name;
            menu.Href = url;

            //Save to xml configmenu
            lstMenu.Add(menu);
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMenu(int id, string name, string url)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");

            var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            menu.Name = name;
            menu.Href = url;

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMenu(int id)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            lstMenu.Remove(menu);

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddChildMenu(int id, string name, string url)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();

            ChildMenu childMenu = new ChildMenu();
            if (menu.LstChildMenu != null && menu.LstChildMenu.Count > 0)
            {
                childMenu.ID = menu.LstChildMenu.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            }
            else
            {
                childMenu.ID = 1;
            }

            childMenu.Name = name;
            childMenu.Href = url;
            menu.LstChildMenu.Add(childMenu);

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditChildMenu(int parentID, int childrenID, string name, string url)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            //Get childmenu
            var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            childMenu.Name = name;
            childMenu.Href = url;

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteChildMenu(int parentID, int childrenID)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            //Get childmenu
            var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            menu.LstChildMenu.Remove(childMenu);

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenu);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMenu(List<int> lstID)
        {
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();

            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml");

            List<Menu> lstMenuNew = new List<Menu>();
            foreach (var itemID in lstID)
            {
                foreach (var itemLayout in lstMenu)
                {
                    if (itemID == itemLayout.ID)
                    {
                        lstMenuNew.Add(new Models.Base.Menu { ID = itemLayout.ID, Name = itemLayout.Name, Href = itemLayout.Href, LstChildMenu = itemLayout.LstChildMenu });
                    }
                }
            }

            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + cpID.ToString() + "/" + theme.Name + "/configmenu.xml", lstMenuNew);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockManager()
        {
            var lstBlock = db.Blocks.Where(m => m.CompanyId == cpID).ToList();
            return View(lstBlock);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlock(string title, string content)
        {
            Models.Block block = new Models.Block();
            block.Name = title;
            block.Content = content;
            block.CompanyId = cpID;
            db.Blocks.Add(block);

            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentBlock(int id)
        {
            var block = db.Blocks.Where(m => m.ID == id && m.CompanyId == cpID).FirstOrDefault();
            return Json(new { Title = block.Name, Content = block.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlock(int id, string title, string content)
        {
            var block = db.Blocks.Where(m=>m.CompanyId == cpID && m.ID ==id).FirstOrDefault();

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
            var block = db.Blocks.Where(m => m.CompanyId == cpID && m.ID == id).FirstOrDefault();

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
        public ActionResult PageManager()
        {
            var lstPage = db.Pages.Where(m=>m.CompanyId == cpID).ToList();
            return View(lstPage);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddPage(string title, string content, bool usingLayout)
        {
            Models.Page page = new Models.Page();
            page.Name = title;
            page.Content = content;
            page.CompanyId = cpID;
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
            var page = db.Pages.Where(m => m.ID == id && m.CompanyId == cpID).FirstOrDefault();

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
            var page = db.Pages.Where(m => m.CompanyId == cpID && m.ID == id).FirstOrDefault();
            return Json(new { Title = page.Name, Content = page.Content, UsingLayout = page.UsingLayout }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeletePage(int id)
        {
            var page = db.Pages.Where(m => m.ID == id && m.CompanyId == cpID).FirstOrDefault();

            db.Pages.Remove(page);
            //Save List Block
            db.SaveChanges();

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlogManager()
        {
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
            blog.Thumb = thumb;
            blog.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarketingManager()
        {
            List<Marketing> lstMarketing = db.GetMarketings.ToList();
            return View(lstMarketing);
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
                        db = new SBS_Entities();
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
        #region Configuration
        public ActionResult ShippingFee()
        {
            var lstShippingFee = db.GetShippingFees.ToList();
            return View(lstShippingFee);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult CreateShippingFee(string name, double cost, string description)
        {
            ShippingFee shFee = new Models.ShippingFee();
            shFee.Name = name;
            shFee.Value = cost;
            shFee.Description = description;
            db.ShippingFees.Add(shFee);
            db.SaveChanges();
            return Json(true);
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult EditShippingFee(int id, string name, double cost, string description)
        {
            var shFee = db.GetShippingFees.Where(m => m.Id == id).FirstOrDefault();
            if (shFee != null)
            {
                shFee.Name = name;
                shFee.Value = cost;
                shFee.Description = description;
                db.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult DeleteShippingFee(int id)
        {
            var shFee = db.GetShippingFees.Where(m => m.Id == id).FirstOrDefault();

            if (shFee != null)
            {
                db.ShippingFees.Remove(shFee);
                db.SaveChanges();
            }

            return Json(true);
        }

        [HttpPost]
        public ActionResult GetShippingFee(int id)
        {
            var shFee = db.GetShippingFees.Where(m => m.Id == id).FirstOrDefault();
            return Json(new { Name = shFee.Name, Value = shFee.Value, Description = shFee.Description });
        }
        [HttpGet]
        public ActionResult ConfigPaypal()
        {
            var configPaypal = db.GetConfigPaypals.FirstOrDefault();
            var configPaypalDTO = AutoMapper.Mapper.Map<ConfigPaypal, ConfigPaypalDTO>(configPaypal);
            return View(configPaypalDTO);
        }
        [HttpPost]
        public ActionResult ConfigPaypal(ConfigPaypalDTO configPaypalDTO)
        {
            try
            {
                ViewBag.Message = " Configuration has been updated successfully.";
                var configPaypal = AutoMapper.Mapper.Map<ConfigPaypalDTO, ConfigPaypal>(configPaypalDTO);
                db.Entry(configPaypal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return View(configPaypalDTO);
            }
            catch (Exception)
            {
                ViewBag.Message = " Configuration has been updated failed.";
                return View(configPaypalDTO);
            }
           
        }
        #endregion

        private const string ClassName = nameof(AdminController);
        private const string PathOrder = "~/Views/Admin/Orders.cshtml";
        private const string PathPartialOrder = "~/Views/Admin/_PartialOrder.cshtml";
        private const string PathPartialDetail = "~/Views/Admin/_PartialOrderDetail.cshtml";
        private const string PathPartialPending = "~/Views/Admin/_PartialPendingOrders.cshtml";
        private const string PathPartialProcessing = "~/Views/Admin/_PartialProcessingOrders.cshtml";
        private const string PathPartialCompleted = "~/Views/Admin/_PartialCompletedOrders.cshtml";
        private const string PathPartialCanceled = "~/Views/Admin/_PartialCanceledOrders.cshtml";
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
                details = db.GetOrderDetails.Where(m => m.OrderId == id).ToList();
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

        private List<Models.Order> ProcessFilter(int kind, int? status, string sort, string dateFrom, string dateTo, int offset = 0, int limit = 100)
        {
            string asc = "asc";
            string desc = "desc";
            List<Models.Order> result = new List<Models.Order>();
            DateTime datefrom;
            DateTime dateto;
            if (kind == (int)OrderStatus.Processing)
            {
                if (sort == asc)
                {
                    result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else if (sort == desc)
                {
                    result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else
                {
                    if (!string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt >= datefrom)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        dateto = Convert.ToDateTime(dateTo);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt <= dateto)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        dateto = Convert.ToDateTime(dateTo);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.ShippingStatus == status && m.CreatedAt >= datefrom && m.CreatedAt <= dateto)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                }
            }
            else
            {
                if (sort == asc)
                {
                    result = db.GetOrders.Where(m => m.OrderStatus == kind).OrderBy(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else if (sort == desc)
                {
                    result = db.GetOrders.Where(m => m.OrderStatus == kind).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                }
                else
                {

                    if (!string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.CreatedAt >= datefrom)
                        .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    {
                        dateto = Convert.ToDateTime(dateTo);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.CreatedAt <= dateto)
                            .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
                    {
                        result = db.GetOrders.Where(m => m.OrderStatus == kind).OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                    else
                    {
                        datefrom = Convert.ToDateTime(dateFrom);
                        dateto = Convert.ToDateTime(dateTo);
                        result = db.GetOrders.Where(m => m.OrderStatus == kind && m.CreatedAt >= datefrom && m.CreatedAt <= dateto)
                            .OrderByDescending(m => m.CreatedAt).Skip(offset).Take(limit).ToList();
                    }
                }
            }
            return result;
        }

        private List<Models.Order> GetOrders(OrderStatus kind)
        {
            List<Models.Order> result = new List<Models.Order>();
            try
            {
                result = db.GetOrders.Where(m => m.OrderStatus == (int)kind).OrderBy(m => m.CreatedAt).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }
    }
}