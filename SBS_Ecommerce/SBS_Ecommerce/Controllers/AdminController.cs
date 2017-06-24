using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
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
using Facebook;
using System.Dynamic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using MailChimp.Net.Interfaces;
using MailChimp.Net;
using MailChimp.Net.Models;
using AutoMapper;
using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Models.ViewModels;
using static SBS_Ecommerce.Models.ResponseResult;

namespace SBS_Ecommerce.Controllers
{
    [CustomAuthorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
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

        private SBSUnitWork unitWork;

        public AdminController()
        {
            unitWork = new SBSUnitWork();
        }

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

            ViewBag.MessageError = SBSMessages.MessageIncorrectLogin;
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
                ViewBag.MessageSuccess = SBSMessages.CheckEmail;
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
            var themes = GetThemes();
            ViewBag.Themes = themes;
            ViewBag.Title = "Theme Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ChangeLayout(List<int> lstID)
        {
            try
            {
                var lstLayout = GetConfigLayouts();
                for (int i = 0; i < lstID.Count; i++)
                {
                    int lid = lstID[i];
                    var layout = lstLayout.FirstOrDefault(m => m.Id == lid);
                    layout.Position = i + 1;
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.ChangeLayoutSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayoutManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            var theme = GetThemeActive();
            var lstLayout = GetConfigLayouts().Where(m => m.Active).OrderBy(m => m.Position).ToList();
            ViewBag.RenderLayout = lstLayout;

            var configchat = GetConfigChatting();
            if (configchat != null)
            {
                ViewBag.PageID = configchat.PageID;
            }

            List<ConfigSlider> sliders = GetConfigSliders();
            ViewBag.RenderSlider = sliders;
            ViewBag.Title = "Layout Manager";
            return View();
        }

        [HttpPost]
        public ActionResult ActiveBlock(int id)
        {
            try
            {
                var layout = GetConfigLayout(id);
                if (layout != null)
                {
                    layout.Active = true;
                    layout.UpdatedAt = DateTime.Now;
                    unitWork.SaveChanges();
                    rs.Message = SBSMessages.AddGadgetSuccess;
                }
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeactiveBlock(int id)
        {
            try
            {
                var layout = GetConfigLayout(id);
                if (layout != null)
                {
                    layout.Active = false;
                    layout.UpdatedAt = DateTime.Now;
                    unitWork.SaveChanges();
                    rs.Message = SBSMessages.RemoveGadgetSuccess;
                }                
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetHTML(int id)
        {
            var layout = GetConfigLayout(id);
            return Json(new { Title = layout?.Name, Content = layout?.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddHTML(string content, string title)
        {
            try
            {
                ConfigLayout layout = new ConfigLayout()
                {
                    CompanyId = cId,
                    Path = "\\Widget\\_PartialHTML.cshtml",
                    Position = GetConfigLayouts().Count + 1,
                    Name = !string.IsNullOrEmpty(title) ? title : "HTML/JavaScript",
                    Content = content,
                    Active = true,
                    CanEdit = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                unitWork.Repository<ConfigLayout>().Add(layout);
                unitWork.SaveChanges();
                rs.Message = SBSMessages.AddGadgetSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditHTML(string content, string title, int id)
        {
            try
            {
                var layout = GetConfigLayout(id);
                if (layout != null)
                {
                    layout.Name = title;
                    layout.Content = content;
                    layout.UpdatedAt = DateTime.Now;
                    unitWork.SaveChanges();
                    rs.Message = SBSMessages.UpdateGadgetSuccess;
                }
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview(string id)
        {
            string[] lstID = id.Split('_');

            var theme = GetThemeActive();
            var lstLayoutPreview = new List<ConfigLayout>();
            var lstLayout = GetConfigLayouts();
            for (int i = 0; i < lstID.Length; i++)
            {
                int lid = int.Parse(lstID[i]);
                var layout = lstLayout.FirstOrDefault(m => m.Id == lid);
                if (layout != null)
                {
                    layout.Position = i + 1;
                    lstLayoutPreview.Add(layout);
                }               
            }

            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            ViewBag.LstBlog = GetBlogs();

            ViewBag.RenderMenu = GetConfigMenus();
            ViewBag.RenderLayout = lstLayoutPreview.OrderBy(m => m.Position).ToList();

            var configChat = GetConfigChatting();
            if (configChat != null)
            {
                ViewBag.PageID = configChat.PageID;
            }
            return View(theme.PathView + "/Index.cshtml");
        }

        public ActionResult PreViewMenu(string id)
        {
            string[] lstID = id.Split('_');
            var theme = GetThemeActive();
            var lstMenuPreview = new List<ConfigMenu>();
            var lstMenu = GetConfigMenus();
            for (int i = 0; i < lstID.Length; i++)
            {
                int mid = int.Parse(lstID[i]);
                var menu = lstMenu.FirstOrDefault(m => m.MenuId == mid);
                menu.Position = i + 1;
                lstMenuPreview.Add(menu);
            }

            var lstLayout = GetConfigLayouts().Where(m => m.Active).ToList();

            ViewBag.LstCategory = SBSCommon.Instance.GetCategories();
            ViewBag.RenderMenu = lstMenuPreview.OrderBy(m => m.Position).ToList();
            ViewBag.LstBlog = GetBlogs();
            ViewBag.RenderLayout = lstLayout;
            ViewBag.Font = theme.CustomFont;
            ViewBag.Color = theme.CustomColor;
            var configChat = GetConfigChatting();
            if (configChat != null)
            {
                ViewBag.PageID = configChat.PageID;
            }
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
                var allThemes = GetThemes();
                foreach (var item in allThemes)
                {
                    if (item.ID == id)
                        item.Active = true;
                    else
                        item.Active = false;
                    unitWork.Repository<Theme>().Update(item);
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SetDefaultThemeSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInforTheme(int id)
        {
            var theme = GetTheme(id);
            if (theme.Description != null)
                return Json(theme.Description, JsonRequestBehavior.AllowGet);
            else
                return Json("", JsonRequestBehavior.AllowGet);
        }

        //public ActionResult DeleteTheme(int id)
        //{
        //    try
        //    {
        //        var theme = db.Themes.Where(m => m.ID == id).FirstOrDefault();
        //        string extractDelete = Server.MapPath("~/") + "/Views/Theme/" + cId.ToString() + "/ExtraTheme/" + theme.Name;

        //        //Delete folder on extra
        //        Directory.Delete(extractDelete, true);

        //        //Delete file .zip
        //        System.IO.File.Delete(extractDelete + ".zip");

        //        //Delete on view
        //        Directory.Delete(Server.MapPath("~/") + "/Views/Theme/" + cId.ToString() + "/" + theme.Name, true);

        //        //Delete on content
        //        Directory.Delete(Server.MapPath("~/") + "/Content/Theme/" + cId.ToString() + "/" + theme.Name, true);

        //        //If theme on active then set themedefault active
        //        if (theme.Active)
        //        {
        //            db.Themes.Where(m => m.Name == "Default" && m.CompanyId == cId).FirstOrDefault().Active = true;
        //        }

        //        //Remove theme in database
        //        db.Themes.Remove(theme);
        //        unitWork.SaveChanges();

        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        return Json(e.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public ActionResult SaveConfigSlider()
        {
            try
            {
                var sliders = GetConfigSliders();
                string pathSlider = Server.MapPath(SBSConstants.PathUploadSlider);
                if (!Directory.Exists(pathSlider))
                {
                    Directory.CreateDirectory(pathSlider);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    int fileSize = file.ContentLength;
                    string fileName = file.FileName;
                    string mimeType = file.ContentType;
                    Stream fileContent = file.InputStream;
                    var id = Request.Files.Keys[i];

                    // save new image
                    var randomName = cId + "_" + CommonUtil.GetNameUnique() + "_" + fileName;
                    var pathSave = pathSlider + randomName;
                    file.SaveAs(pathSave);

                    // remove old image
                    var slider = sliders.FirstOrDefault(m => m.Id == int.Parse(id));
                    if (slider != null)
                    {
                        if (!string.IsNullOrEmpty(slider.Path))
                        {
                            CommonUtil.DeleteFile(Server.MapPath(slider.Path));
                        }
                        slider.Path = SBSConstants.PathUploadSlider + randomName;
                    }
                }

                //Remove file and path
                var idRemove = System.Web.HttpContext.Current.Request.Form["id"];
                if (idRemove != null)
                {
                    var lstId = idRemove.Split(',');
                    foreach (var item in lstId)
                    {
                        //Remove file if exist
                        var old = sliders.FirstOrDefault(m => m.Id == int.Parse(item));
                        CommonUtil.DeleteFile(Server.MapPath(old?.Path));
                        old.Path = string.Empty;
                    }
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.ChangeSliderSuccess;
            }
            catch(Exception e)
            {
                rs.Status = SBSConstants.Failed;
                rs.Message = e.Message;
            }
            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MenuManager()
        {
            var menu = GetConfigMenus().OrderBy(m => m.Position).ToList();
            ViewBag.Title = "Menu Manager";
            ViewBag.LstMenu = menu;
            ViewBag.Pages = GetPages();
            return View();
        }

        [HttpPost]
        public ActionResult AddMenu(string name, string url)
        {
            if (string.IsNullOrEmpty(name))
            {
                CheckErrorStates();
                rs.ErrorStates.Add(new ErrorState { Key = "Name", Value = "The Menu Name is required" });
            }
            if (string.IsNullOrEmpty(url))
            {
                CheckErrorStates();
                rs.ErrorStates.Add(new ErrorState { Key = "Url", Value = "The Url is required" });
            }
            else
            {
                try
                {
                    ConfigMenu menu = new ConfigMenu()
                    {
                        CompanyId = cId,
                        Name = name,
                        Href = url,
                        Position = GetConfigMenus().Count + 1,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    unitWork.Repository<ConfigMenu>().Add(menu);
                    unitWork.SaveChanges();
                    rs.Message = SBSMessages.AddMenuSuccess;
                }
                catch (Exception e)
                {
                    SetResponseStatus(e);
                }
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditMenu(int id, string name, string url)
        {
            try {
                var menu = GetConfigMenu(id);
                if (menu != null)
                {
                    menu.Name = name;
                    menu.Href = url;
                    menu.UpdatedAt = DateTime.Now;
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.UpdateMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteMenu(int id)
        {
            try
            {
                var menu = new ConfigMenu { MenuId = id };
                if (menu != null)
                {
                    unitWork.Repository<ConfigMenu>().Delete(menu);
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.DeleteMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddChildMenu(int id, string name, string url)
        {
            try
            {
                var menu = GetConfigMenu(id);
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
                    unitWork.Repository<ConfigChildMenu>().Add(childmenu);
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.AddChildMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditChildMenu(int parentID, int childrenID, string name, string url)
        {
            try
            {
                var childmenu = GetConfigChildMenu(childrenID);
                if (childmenu != null)
                {
                    childmenu.Name = name;
                    childmenu.Href = url;
                    childmenu.UpdatedAt = DateTime.Now;
                    unitWork.Repository<ConfigChildMenu>().Update(childmenu);
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.UpdateChildMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteChildMenu(int parentID, int childrenID)
        {
            try
            {
                var childmenu = new ConfigChildMenu { Id = childrenID };
                if (childmenu != null)
                {
                    unitWork.Repository<ConfigChildMenu>().Delete(childmenu);
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.DeleteChildMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMenu(List<int> lstID)
        {
            try
            {
                for (int i = 0; i < lstID.Count; i++)
                {
                    var id = lstID[i];
                    var menu = GetConfigMenu(id);
                    menu.Position = i + 1;
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.UpdateMenuSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlockManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }
            var lstBlock = unitWork.Repository<Block>().GetAll(m => m.CompanyId == cId).ToList();
            return View(lstBlock);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlock(string title, string content)
        {
            try
            {
                Block block = new Block();
                block.Name = title;
                block.Content = content;
                block.CompanyId = cId;
                unitWork.Repository<Block>().Add(block);

                unitWork.SaveChanges();
                rs.Message = SBSMessages.AddBlockSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentBlock(int id)
        {
            var block = GetBlock(id);
            return Json(new { Title = block.Name, Content = block.Content }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlock(int id, string title, string content)
        {
            try
            {
                var block = GetBlock(id);

                block.Name = title;
                block.Content = content;

                unitWork.Repository<Block>().Update(block);
                unitWork.SaveChanges();
                rs.Message = SBSMessages.UpdateBlockSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Delete block
        /// </summary>
        /// <param name="id">Block ID</param>
        /// <returns>Status</returns>
        [HttpPost]
        public ActionResult DeleteBlock(int id)
        {
            try
            {
                unitWork.Repository<Block>().Delete(new Block { ID = id });
                unitWork.SaveChanges();
                rs.Message = SBSMessages.DeleteBlockSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Page Manager
        /// </summary>
        /// <returns>View</returns>
        public ActionResult PageManager()
        {
            return View(GetPages());
        }

        [HttpPost]
        public ActionResult CheckDuplicateNamePage(string name, int? id)
        {
            Page page = unitWork.Repository<Page>().Get(m => m.Name.ToUpper() == name.ToUpper());
            if (page != null)
            {
                if (id != null && page.ID == id)
                    return Json(true);
                else
                    return Json(false);
            }         
            return Json(true);
        }

        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageViewModel model;
            var page = GetPage(id);
            model = Mapper.Map<Page, PageViewModel>(page);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> AddOrUpdatePage(PageViewModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.CompanyId = cId;
                model.UsingLayout = true;
                var page = Mapper.Map<PageViewModel, Page>(model);
                if (page.ID == 0)
                {
                    unitWork.Repository<Page>().Add(page);
                    SetTempDataMessage(SBSMessages.AddPageSuccess);
                }
                else
                {
                    unitWork.Repository<Page>().Update(page);
                    SetTempDataMessage(SBSMessages.UpdatePageSuccess);
                }
                await unitWork.SaveChangesAsync();                
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("PageManager");
        }

        [HttpPost]
        public ActionResult DeletePage(int id)
        {
            try
            {
                unitWork.Repository<Page>().Delete(new Page { ID = id });
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.DeletePageSuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("PageManager");
        }

        public ActionResult BlogManager(string msg, string textMsg)
        {
            if (!string.IsNullOrEmpty(msg) && !string.IsNullOrEmpty(textMsg))
            {
                ViewBag.Message = msg;
                ViewBag.TextMessage = textMsg;
            }

            var lstBlog = GetBlogs();
            return View(lstBlog);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddBlog(string title, string content, string path)
        {
            try
            {
                Blog blog = new Blog();
                blog.CompanyId = cId;
                blog.Title = title;
                blog.BlogContent = content;
                blog.CreatedAt = DateTime.Now;
                blog.UpdatedAt = DateTime.Now;
                blog.Status = "1";
                blog.Thumb = path;
                unitWork.Repository<Blog>().Add(blog);
                unitWork.SaveChanges();
                rs.Message = SBSMessages.AddBlogSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteBlog(int id)
        {
            try
            {
                Blog blog = GetBlog(id);
                CommonUtil.DeleteFile(Server.MapPath(blog?.Thumb));
                unitWork.Repository<Blog>().Delete(blog);
                unitWork.SaveChanges();
                rs.Message = SBSMessages.DeleteBlogSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadImageThumbnail()
        {
            var path = SBSConstants.PathUploadBlog;
            //Uploaded file
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                Stream fileContent = file.InputStream;

                //Path content of theme
                var pathContentofTheme = Server.MapPath(SBSConstants.PathUploadBlog);

                //To save file, use SaveAs method
                var randomName = cId + "_" + CommonUtil.GetNameUnique() + "_" + fileName;
                string pathSave = pathContentofTheme + randomName;
                path = path + randomName;
                file.SaveAs(pathSave); //File will be saved in application root
            }
            //Return status
            return Json(path, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetContentBlog(int id)
        {
            Blog blog = GetBlog(id);
            var thumb = "";
            if (!string.IsNullOrEmpty(blog.Thumb))
                thumb = Url.Content(blog.Thumb);
            else
                thumb = blog.Thumb;
            return Json(new { Title = blog.Title, Content = blog.BlogContent, Thumb = thumb }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditBlog(int id, string title, string content, string thumb)
        {
            try
            {
                Blog blog = GetBlog(id);
                if (blog != null)
                {
                    blog.Title = title;
                    blog.BlogContent = content;
                    if (thumb != "nochange")
                    {
                        CommonUtil.DeleteFile(Server.MapPath(blog?.Thumb));
                        blog.Thumb = thumb;
                    }

                    blog.UpdatedAt = DateTime.Now;
                    unitWork.SaveChanges();
                }
                rs.Message = SBSMessages.UpdateBlogSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult MarketingManager()
        //{
        //    List<Marketing> lstMarketing = db.GetMarketings.ToList();
        //    return View(lstMarketing);
        //}

        public async Task<ActionResult> PushToMailChimp()
        {
            // Instantiate new manager
            // Get apiKey
            //var apiKey = db.ConfigMailChimps.FirstOrDefault();
            var apiKey = GetConfigMailChimp();
            if (apiKey != null)
            {

                IMailChimpManager mailChimpManager = new MailChimpManager(apiKey.ApiKey);
                var mailChimpListCollection = await mailChimpManager.Lists.GetAllAsync().ConfigureAwait(false);
                var listId = mailChimpListCollection.FirstOrDefault().Id;
                var x = "\"";
                // Use the Status property if updating an existing member
                var lstUser = unitWork.Repository<User>().GetAll(m => (m.PushMailChimp == null || m.PushMailChimp == false) && m.CompanyId == cId).ToList();

                foreach (var item in lstUser)
                {
                    //Update status mailchimp
                    var user = unitWork.Repository<User>().Get(m => m.Id == item.Id);
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
                    unitWork.SaveChanges();
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
            var lstUser = unitWork.Repository<User>().GetAll(m => m.CompanyId == cId).ToList();
            return View(lstUser);
        }


        public ActionResult SendMailManager(int id)
        {
            ViewBag.IDMarketing = id;
            List<ScheduleEmail> lstScheduleEmail = unitWork.Repository<ScheduleEmail>().GetAll(m => m.MarketingID == id && m.CompanyId == cId).ToList();
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
            unitWork.Repository<Marketing>().Add(marketing);

            //Save change
            unitWork.SaveChanges();

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
            var marketing = GetMarketing(id);

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
            var campaign = GetMarketing(id);
            if (campaign != null)
            {
                campaign.NameCampain = name;
                campaign.Content = content;
                unitWork.SaveChanges();
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
            var campaign = GetMarketing(id);
            if (campaign != null)
            {
                //db.Marketings.Remove(campaign);
                unitWork.Repository<Marketing>().Delete(campaign);
                unitWork.SaveChanges();
            }

            //Return status update
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmailSystem()
        {
            var email = "";
            //foreach (var item in db.Users)
            foreach (var item in unitWork.Repository<User>().GetAll().ToList())
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
            var campaign = GetMarketing(id);
            if (campaign != null)
            {
                if (campaign.Status != null && (bool)campaign.Status)
                    campaign.Status = false;
                else
                    campaign.Status = true;
                unitWork.SaveChanges();
            }

            //Return status update
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchedual(int id)
        {
            var chEmail = GetSchedulerEmail(id);
            return Json(new { Email = chEmail.Email, Subject = chEmail.Subject, Schedule = chEmail.Schedule }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSchedual(int id)
        {
            var chEmail = GetSchedulerEmail(id);
            if (chEmail != null)
            {
                unitWork.Repository<ScheduleEmail>().Delete(chEmail);
                unitWork.SaveChanges();
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SendMail(int id, string time, List<string> lstEmail, string subject)
        {
            try
            {
                var emailmarketing = GetMarketing(id);
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
                unitWork.Repository<ScheduleEmail>().Add(schEmail);
                unitWork.SaveChanges();

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
            var scEmail = GetSchedulerEmail(id);
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

                scEmail.Email = string.Join(" ", lstEmail).Trim();
                scEmail.Schedule = datetime;
                scEmail.Subject = subject;
                unitWork.SaveChanges();
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
                var schEmail = GetSchedulerEmail(id);
                if (DateTime.Now > schEmail.Schedule)
                {
                    //To do
                    EmailUtil emailUT = new EmailUtil();
                    emailUT.SendListEmail(message);
                    schEmail.Status = true;
                    unitWork.SaveChanges();
                }
                else
                {
                    while (DateTime.Now < schEmail.Schedule)
                    {
                        schEmail = GetSchedulerEmail(id);
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
                    unitWork.SaveChanges();
                }
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
            return View(GetConfigChatting());
        }

        public ActionResult SaveConfigMailChimp(string apiKey)
        {
            try
            {
                var configMailChimp = GetConfigMailChimp();
                if (configMailChimp == null && !string.IsNullOrEmpty(apiKey))
                {
                    configMailChimp = new ConfigMailChimp();
                    configMailChimp.CompanyId = cId;
                    configMailChimp.ApiKey = apiKey;
                    unitWork.Repository<ConfigMailChimp>().Add(configMailChimp);
                }
                else
                {
                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        configMailChimp.ApiKey = apiKey;
                    }
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SetMailchimpSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveConfigChatting(string pageID)
        {
            try
            {
                var cfChatting = GetConfigChatting();
                if (cfChatting == null && !string.IsNullOrEmpty(pageID))
                {
                    cfChatting = new ConfigChatting();
                    cfChatting.PageID = pageID;
                    cfChatting.PathPage = pageID;
                    unitWork.Repository<ConfigChatting>().Add(cfChatting);
                }
                else
                {
                    if (!string.IsNullOrEmpty(pageID))
                        cfChatting.PageID = pageID;
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SetChattingSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BlogComment(int id)
        {
            var lstBlogComment = unitWork.Repository<BlogComment>().GetAll(m => m.CompanyId == cId && m.BlogId == id).ToList();
            return View(lstBlogComment);
        }

        [HttpPost]
        public ActionResult DeleteBlogComment(int id)
        {
            try
            {
                unitWork.Repository<BlogComment>().Delete(new BlogComment { Id = id });
                unitWork.SaveChanges();
                rs.Message = SBSMessages.DeleteBlogCommentSuccess;
            }
            catch(Exception e)
            {
                SetResponseStatus(e);
            }
            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        #region Configuration
        public ActionResult ShippingFee()
        {
            try
            {
                ViewBag.Countries = CountryUtil.Instance.GetCountries();
                ViewBag.WeightBaseds = unitWork.Repository<WeightBased>().GetAll(m => m.CompanyId == cId).ToList();
                ViewBag.LocalPickup = unitWork.Repository<LocalPickup>().Get(m => m.CompanyId == cId);
                ViewBag.DeliveryCompanies = GetDeliveryCompanies();
                ViewBag.WeightBasedEnable = unitWork.Repository<ConfigShipping>().Get(m => m.CompanyId == cId && m.Name.Contains("Weight Based"));
                ViewBag.LocalPickupEnable = unitWork.Repository<ConfigShipping>().Get(m => m.CompanyId == cId && m.Name.Contains("Local Pickup"));
                ViewBag.UnitOfMass = unitOfMass;
            }
            catch
            {

            }
            return View();
        }

        [HttpGet]
        public ActionResult AddWeightBased()
        {
            ViewBag.Countries = CountryUtil.Instance.GetCountries();
            ViewBag.DeliveryCompanies = GetDeliveryCompanies();
            ViewBag.UnitOfMass = unitOfMass;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InsertOrUpdateWeightBased(WeightBasedViewModel model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.CompanyId = cId;
                var wb = Mapper.Map<WeightBasedViewModel, WeightBased>(model);
                if (model.Id == 0)
                {
                    model.CreatedAt = DateTime.Now;
                    unitWork.Repository<WeightBased>().Add(wb);
                    SetTempDataMessage(SBSMessages.AddWeightBasedSuccess);
                }
                else
                {
                    model.UpdatedAt = DateTime.Now;
                    unitWork.Repository<WeightBased>().Update(wb);
                    SetTempDataMessage(SBSMessages.UpdateWeightBasedSuccess);
                }                
                unitWork.SaveChanges();
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("ShippingFee");
        }

        public ActionResult Configuration()
        {
            var configPaypal = GetConfigPaypal();
            var configPaypalDTO = Mapper.Map<ConfigPaypal, ConfigPaypalDTO>(configPaypal);
            var configEmail = unitWork.Repository<EmailAccount>().Get(m => m.CompanyId == cId);
            var configEmailDTO = Mapper.Map<EmailAccount, EmailAccountDTO>(configEmail);
            var configMailChimp = GetConfigMailChimp();

            ViewBag.ConfigChatting = GetConfigChatting();
            ViewBag.ConfigPaypalDTO = configPaypalDTO;
            ViewBag.ConfigEmail = configEmailDTO;
            ViewBag.ConfigMailChimp = configMailChimp;
            return View();
        }

        [HttpPost]
        public ActionResult ConfigPaypal(string Id, string Mode, int? ConnectionTimeout, string ClientId, string ClientSecret)
        {
            try
            {
                ConfigPaypal configPaypal = new ConfigPaypal();
                configPaypal.CompanyId = cId;
                configPaypal.Mode = Mode;
                configPaypal.ConnectionTimeout = ConnectionTimeout;
                configPaypal.ClientId = ClientId;
                configPaypal.ClientSecret = ClientSecret;

                if (!string.IsNullOrEmpty(Id))
                {
                    configPaypal.Id = int.Parse(Id);
                    unitWork.Repository<ConfigPaypal>().Update(configPaypal);
                }
                else
                {
                    unitWork.Repository<ConfigPaypal>().Add(configPaypal);
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SetPaypalSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfigEmail(string Id, string Email, string DisplayName, string Host, int Port, string Username, string Password, bool EnableSsl, bool UseDefaultCredentials)
        {
            EmailAccount emailAccount = new EmailAccount();
            emailAccount.Host = Host;
            emailAccount.DisplayName = DisplayName;
            emailAccount.Email = Email;
            emailAccount.EnableSsl = EnableSsl;
            emailAccount.Password = Password;
            emailAccount.Port = Port;
            emailAccount.UseDefaultCredentials = UseDefaultCredentials;
            emailAccount.Username = Username;

            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    emailAccount.Id = int.Parse(Id);
                    unitWork.Repository<EmailAccount>().Update(emailAccount);
                }
                else
                {
                    unitWork.Repository<EmailAccount>().Add(emailAccount);
                }
                unitWork.SaveChanges();
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }            
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// Get Orders.
        /// </summary>
        /// <returns></returns>
        public ActionResult Orders(int? kind, string startDate, string endDate, string textSearch)
        {
            try
            {
                ViewBag.Kind = kind;
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                ViewBag.TextSearch = textSearch;
                switch (kind)
                {
                    case (int)OrderStatus.Delivering:
                        ViewBag.Data = GetOrders(kind, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Delivering.ToString();
                        break;
                    case (int)OrderStatus.Processing:
                        ViewBag.Data = GetOrders(kind, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Processing.ToString();
                        break;
                    case (int)OrderStatus.Completed:
                        ViewBag.Data = GetOrders(kind, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Completed.ToString();
                        break;
                    case (int)OrderStatus.Cancelled:
                        ViewBag.Data = GetOrders(kind, startDate, endDate, textSearch);
                        ViewBag.Tag = OrderStatus.Completed.ToString();
                        break;
                    default:
                        ViewBag.Data = GetOrders(kind, startDate, endDate, textSearch);
                        ViewBag.Tag = "All Orders";
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
                var data = unitWork.Repository<Models.Order>().Find(id);
                ViewBag.BillingAddress = unitWork.Repository<UserAddress>().Find(data.BillingAddressId);
                ViewBag.ShippingAddress = unitWork.Repository<UserAddress>().Find(data.ShippingAddressId);
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
            try
            {
                var order = unitWork.Repository<Models.Order>().Find(id);
                switch (order.OrderStatus)
                {
                    case (int)OrderStatus.Reserved:
                        flag = true;
                        order.OrderStatus = (int)OrderStatus.Processing;
                        order.ShippingStatus = (int)ShippingStatus.NotYetShipped;
                        break;
                    case (int)OrderStatus.Processing:
                        flag = true;
                        order.OrderStatus = (int)OrderStatus.Delivering;
                        order.ShippingStatus = (int)ShippingStatus.NotYetShipped;
                        break;
                    case (int)OrderStatus.Delivering:
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
                    await unitWork.SaveChangesAsync();
                }
                rs.Message = SBSMessages.ChangeStatusSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Call api update quanlity on store 
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        private async Task<int> StockOut(string orderId)
        {
            ListStockOutDTO lstStockOutDTO = new ListStockOutDTO();
            var lstOrderDetail = unitWork.Repository<OrderDetail>().GetAll(o => o.OrderId == orderId).ToList();
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
            ViewBag.Data = GetDeliveryCompanies();
            return View("~/Views/Admin/DeliveryCompanyManager.cshtml");
        }

        [HttpGet]
        public ActionResult AddDeliveryCompany()
        {
            ViewBag.Countries = CountryUtil.Instance.GetCountries();
            return View();
        }

        [HttpPost]
        public ActionResult InsertOrUpdateDeliveryCompany(DeliveryCompanyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.CompanyId = cId;
                var dc = Mapper.Map<DeliveryCompanyViewModel, DeliveryCompany>(model);
                if (dc.Id == 0)
                {
                    dc.CreatedAt = DateTime.Now;
                    dc.UpdatedAt = DateTime.Now;
                    unitWork.Repository<DeliveryCompany>().Add(dc);
                    SetTempDataMessage(SBSMessages.AddDeliveryCompanySuccess);
                }
                else
                {
                    dc.UpdatedAt = DateTime.Now;
                    unitWork.Repository<DeliveryCompany>().Update(dc);
                    SetTempDataMessage(SBSMessages.UpdateDeliveryCompanySuccess);
                }
                unitWork.SaveChanges();                
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("DeliveryCompany");
        }

        [HttpPost]
        public ActionResult DeleteDeliveryCompany(int id)
        {
            try
            {
                unitWork.Repository<DeliveryCompany>().Delete(new DeliveryCompany() { Id = id });
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.DeleteDeliveryCompanySuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("DeliveryCompany");
        }

        [HttpGet]
        public ActionResult EditDeliveryCompany(int id)
        {
            ViewBag.Countries = CountryUtil.Instance.GetCountries();
            var dc = unitWork.Repository<DeliveryCompany>().Find(id);
            var model = Mapper.Map<DeliveryCompany, DeliveryCompanyViewModel>(dc);
            return View(model);
        }

        [HttpPost]
        public ActionResult DuplicateWeightBase(int id)
        {
            try
            {
                var item = unitWork.Repository<WeightBased>().Find(id);
                if (item != null)
                {
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

                    unitWork.Repository<WeightBased>().Add(clone);
                    unitWork.SaveChanges();
                }
                SetTempDataMessage(SBSMessages.DuplicateWeightBasedSuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);                
            }
            return RedirectToAction("ShippingFee");
        }

        [HttpGet]
        public ActionResult EditWeightBased(int id)
        {
            ViewBag.DeliveryCompanies = GetDeliveryCompanies();
            ViewBag.Countries = CountryUtil.Instance.GetCountries();
            var wb = unitWork.Repository<WeightBased>().Find(id);
            ViewBag.UnitOfMass = unitOfMass;
            var model = Mapper.Map<WeightBased, WeightBasedViewModel>(wb);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateWeighBasedConfiguration()
        {
            try
            {
                var item = unitWork.Repository<ConfigShipping>().Get(m => m.CompanyId == cId && m.Name.Contains("Weight Based"));
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
                    unitWork.Repository<ConfigShipping>().Add(item);
                }
                else
                {
                    if ((bool)item?.Status)
                        item.Status = false;
                    else
                        item.Status = true;

                    item.UpdatedAt = DateTime.Now;
                    unitWork.Repository<ConfigShipping>().Update(item);
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SettingShippingFeeSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteWeightBased(int id)
        {
            try
            {
                WeightBased item = new WeightBased() { Id = id };
                unitWork.Repository<WeightBased>().Delete(item);
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.DeleteWeightBasedSuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("ShippingFee");
        }

        [HttpPost]
        public ActionResult UpdateLocalPickupConfiguration()
        {
            try
            {
                var item = unitWork.Repository<ConfigShipping>().Get(m => m.CompanyId == cId && m.Name.Contains("Local Pickup"));
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
                    unitWork.Repository<ConfigShipping>().Add(item);
                }
                else
                {
                    if ((bool)item?.Status)
                        item.Status = false;
                    else
                        item.Status = true;
                    item.UpdatedAt = DateTime.Now;
                    unitWork.Repository<ConfigShipping>().Update(item);
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SettingLocalPickupSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateLocalPickupInfo(LocalPickup model)
        {
            try
            {
                model.CompanyId = cId;
                if (model.Id == 0)
                {
                    model.CreatedAt = DateTime.Now;
                    model.UpdatedAt = DateTime.Now;
                    unitWork.Repository<LocalPickup>().Add(model);
                }
                else
                {
                    model.UpdatedAt = DateTime.Now;
                    unitWork.Repository<LocalPickup>().Update(model);
                }
                unitWork.SaveChanges();
                rs.Message = SBSMessages.SettingLocalPickupSuccess;
            }
            catch (Exception e)
            {
                SetResponseStatus(e);
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public ActionResult DeliveryScheduler()
        {
            ViewBag.Data = unitWork.Repository<DeliveryScheduler>().GetAll(m => m.CompanyId == cId).ToList();
            return View(Url.Content(PathDeliveryScheduler));
        }

        [HttpGet]
        public ActionResult AddDeliveryScheduler()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EditDeliveryScheduler(int id)
        {
            var deliveryScheduler = unitWork.Repository<DeliveryScheduler>().Find(id);
            var model = Mapper.Map<DeliveryScheduler, DeliverySchedulerViewModel>(deliveryScheduler);
            return View(model);
        }

        [HttpPost]
        public ActionResult InsertOrUpdateDeliveryScheduler(DeliverySchedulerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.CompanyId = cId;
                model.TimeSlot = string.IsNullOrEmpty(model.TimeSlot) ? model.FromHour + " - " + model.ToHour : model.TimeSlot;
                var deliveryScheduler = Mapper.Map<DeliverySchedulerViewModel, DeliveryScheduler>(model);
                if (deliveryScheduler.Id == 0)
                {
                    deliveryScheduler.CreatedAt = DateTime.Now;
                    deliveryScheduler.UpdatedAt = DateTime.Now;
                    unitWork.Repository<DeliveryScheduler>().Add(deliveryScheduler);
                    SetTempDataMessage(SBSMessages.AddDeliverySchedulerSuccess);
                }
                else
                {
                    deliveryScheduler.UpdatedAt = DateTime.Now;
                    unitWork.Repository<DeliveryScheduler>().Update(deliveryScheduler);
                    SetTempDataMessage(SBSMessages.UpdateDeliverySchedulerSuccess);
                }
                unitWork.SaveChanges();
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("DeliveryScheduler");
        }

        [HttpPost]
        public ActionResult DeleteDeliveryScheduler(int id)
        {
            try
            {
                unitWork.Repository<DeliveryScheduler>().Delete(new DeliveryScheduler() { Id = id });
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.DeleteDeliverySchedulerSuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message);
            }
            return RedirectToAction("DeliveryScheduler");
        }

        #region Configuration Holiday
        public ActionResult HolidayManager(int? id)
        {
            ViewBag.Year = GetListYear(id);
            List<ConfigHoliday> lstConfigHoliday = unitWork.Repository<ConfigHoliday>().GetAll(m => m.CompanyId == cId).ToList();
            if (lstConfigHoliday == null)
            {
                lstConfigHoliday = new List<ConfigHoliday>();
            }
            if (id != null)
            {
                lstConfigHoliday = lstConfigHoliday.Where(c => c.HolidayDate != null && c.HolidayDate.Value.Year == id).ToList();
            }
            return View(lstConfigHoliday);
        }

        [HttpGet]
        public ActionResult AddHoliday()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddHoliday(ConfigHolidayViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var configHoliday = Mapper.Map<ConfigHolidayViewModel, ConfigHoliday>(model);
                configHoliday.CompanyId = cId;
                configHoliday.CreateAt = DateTime.Now;
                unitWork.Repository<ConfigHoliday>().Add(configHoliday);
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.MessageAddHolidaySuccess);
            } 
            catch(Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }            
            return RedirectToAction("HolidayManager");
        }

        [HttpGet]
        public ActionResult EditHoliday(int id)
        {
            var holiday = unitWork.Repository<ConfigHoliday>().Find(id);
            var configHoliday = Mapper.Map<ConfigHoliday, ConfigHolidayViewModel>(holiday);
            return View(configHoliday);
        }

        [HttpPost]
        public ActionResult EditHoliday(ConfigHolidayViewModel model, bool? IsActive)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var configHoliday = Mapper.Map<ConfigHolidayViewModel, ConfigHoliday>(model);
                configHoliday.CompanyId = cId;
                configHoliday.IsActive = (bool)IsActive;
                configHoliday.UpdateAt = DateTime.Now;
                unitWork.Repository<ConfigHoliday>().Update(configHoliday);
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.MessageUpdatedHolidaySuccess);
            }
            catch(Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }            
            return RedirectToAction("HolidayManager");
        }

        [HttpPost]
        public ActionResult DeleteHoliday(int id)
        {
            try
            {
                unitWork.Repository<ConfigHoliday>().Delete(new ConfigHoliday { Id = id });
                unitWork.SaveChanges();
                SetTempDataMessage(SBSMessages.MessageDeleteHolidaySuccess);
            }
            catch(Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("HolidayManager");
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
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = true });
                    else
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = false });
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    if (year - i == id)
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = true });
                    else
                        items.Add(new SelectListItem { Text = (year - i).ToString(), Value = (year - i).ToString(), Selected = false });
                }
            }
            return items;
        }
        #endregion

        private List<Models.Order> GetOrders(int? kind, string startDate, string endDate, string textSearch)
        {
            List<Models.Order> result = new List<Models.Order>();
            DateTime start;
            DateTime end;
            string dateFormat = "yyyy-MM-dd";
            try
            {
                if (kind == null)
                {
                    // All fields contain values
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                    {
                        start = DateTime.ParseExact(startDate, dateFormat, null);
                        end = DateTime.ParseExact(endDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && m.CreatedAt >= start && m.CreatedAt <= end &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && m.CreatedAt >= start && m.CreatedAt <= end
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Start date and Text search have values
                    else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                    {
                        start = DateTime.ParseExact(startDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && m.CreatedAt >= start &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && m.CreatedAt <= end &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
                            m.CreatedAt >= start
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Only End date has value
                    else if (!string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(textSearch))
                    {
                        end = DateTime.ParseExact(endDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && m.CreatedAt <= end
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Only Text search has value
                    else if (!string.IsNullOrEmpty(textSearch) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                    {
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.CompanyId == cId && (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                            m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                            m.CountProduct.ToString().Contains(textSearch))
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Nothing
                    else
                    {
                        result = unitWork.Repository<Models.Order>().GetAll(m => m.CompanyId == cId)
                            .OrderBy(m => m.CreatedAt).ToList();
                    }
                }
                else
                {
                    // All fields contain values
                    if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                    {
                        start = DateTime.ParseExact(startDate, dateFormat, null);
                        end = DateTime.ParseExact(endDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
                            m.CreatedAt >= start && m.CreatedAt <= end
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Start date and Text search have values
                    else if (!string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(textSearch))
                    {
                        start = DateTime.ParseExact(startDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
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
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
                            m.CreatedAt >= start
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Only End date has value
                    else if (!string.IsNullOrEmpty(endDate) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(textSearch))
                    {
                        end = DateTime.ParseExact(endDate, dateFormat, null);
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
                            m.CreatedAt <= end
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Only Text search has value
                    else if (!string.IsNullOrEmpty(textSearch) && string.IsNullOrEmpty(startDate) && string.IsNullOrEmpty(endDate))
                    {
                        result = unitWork.Repository<Models.Order>().GetAll(
                            m => m.OrderStatus == (int)kind && m.CompanyId == cId &&
                            (m.OrderId.Contains(textSearch) || m.CreatedAt.ToString().Contains(textSearch) ||
                            m.TotalAmount.ToString().Contains(textSearch) || m.Currency.Contains(textSearch) ||
                            m.CountProduct.ToString().Contains(textSearch)
                            )
                        ).OrderBy(m => m.CreatedAt).ToList();
                    }
                    // Nothing
                    else
                    {
                        result = unitWork.Repository<Models.Order>().GetAll(m => m.OrderStatus == (int)kind && m.CompanyId == cId)
                            .OrderBy(m => m.CreatedAt).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return result;
        }

        //public ActionResult SEO()
        //{
        //    return View(unitWork.Repository<SEO>().GetAll(m => m.CompanyId == cId).ToList());
        //}

        //public ActionResult GetSEO(int id)
        //{
        //    try
        //    {
        //        rs.Html = PartialViewToString(this, PathPartialSEODetail, unitWork.Repository<SEO>().Find(id));
        //    }
        //    catch (Exception e)
        //    {
        //        SetResponseStatus(e);
        //    }
        //    return Json(rs, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult AddOrUpdateSEO(SEO model)
        //{
        //    try
        //    {
        //        model.CompanyId = cId;
        //        if (model.Id != 0)
        //        {
        //            model.UpdatedAt = DateTime.Now;
        //            unitWork.Repository<SEO>().Update(model);
        //            rs.Message = SBSMessages.UpdateSEOSuccess;
        //        }
        //        else
        //        {
        //            model.CreatedAt = DateTime.Now;
        //            model.UpdatedAt = DateTime.Now;
        //            unitWork.Repository<SEO>().Add(model);
        //            rs.Message = SBSMessages.AddSEOSuccess;
        //        }
        //        unitWork.SaveChanges();
        //    }
        //    catch (Exception e)
        //    {
        //        SetResponseStatus(e);
        //    }
        //    return Json(rs, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult DeleteSEO(int id)
        //{
        //    try
        //    {
        //        unitWork.Repository<SEO>().Delete(new SEO() { Id = id });
        //        unitWork.SaveChanges();
        //        rs.Message = SBSMessages.DeleteSEOSuccess;
        //    }
        //    catch (Exception e)
        //    {
        //        SetResponseStatus(e);
        //    }
        //    return Json(rs, JsonRequestBehavior.AllowGet);
        //}

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

        public ActionResult CustomeTheme()
        {
            var lstFonts = new List<string>();
            var folder = Server.MapPath("~/Content/custom/fonts");
            string[] files = Directory.GetFiles(folder);
            foreach (var item in files)
            {
                string font = Path.GetFileName(item).Replace(".css", "");
                if (font.Contains("_"))
                {
                    font = font.Replace("_", " ");
                }
                lstFonts.Add(font);
            }
            ViewBag.Font = GetThemeActive().CustomFont;
            ViewBag.Fonts = lstFonts;
            ViewBag.LstMenu = GetConfigMenus().OrderBy(m => m.Position).ToList();
            ViewBag.Pages = GetPages();
            return View();
        }

        public async Task<ActionResult> SaveCustom(string font, string color)
        {
            if (string.IsNullOrEmpty(font))
            {
                SetTempDataMessage(SBSMessages.InvalidFont, SBSConstants.Failed);
                return RedirectToAction("CustomeTheme");
            }
            //if (string.IsNullOrEmpty(color))
            //{
            //    SetTempDataMessage(SBSMessages.InvalidColor, SBSConstants.Failed);
            //    return RedirectToAction("CustomeTheme");
            //}

            try
            {
                var theme = GetThemeActive();
                theme.CustomFont = font;
                //theme.CustomColor = color;
                unitWork.Repository<Theme>().Update(theme);
                await unitWork.SaveChangesAsync();
                SetTempDataMessage(SBSMessages.ChangeCustomSuccess);
            }
            catch (Exception e)
            {
                SetTempDataMessage(e.Message, SBSConstants.Failed);
            }
            return RedirectToAction("CustomeTheme");
        }

        private List<ConfigLayout> GetConfigLayouts()
        {
            return unitWork.Repository<ConfigLayout>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private List<ConfigSlider> GetConfigSliders()
        {
            return unitWork.Repository<ConfigSlider>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private ConfigLayout GetConfigLayout(int id)
        {
            return unitWork.Repository<ConfigLayout>().Get(m => m.Id == id);
        }

        private List<Blog> GetBlogs()
        {
            return unitWork.Repository<Blog>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private Blog GetBlog(int id)
        {
            return unitWork.Repository<Blog>().Get(m => m.BlogId == id);
        }

        private List<Page> GetPages()
        {
            return unitWork.Repository<Page>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private Page GetPage(int? id)
        {
            return unitWork.Repository<Page>().Get(m => m.ID == id);
        }

        private Block GetBlock(int id)
        {
            return unitWork.Repository<Block>().Get(m => m.ID == id);
        }

        private Marketing GetMarketing(int id)
        {
            return unitWork.Repository<Marketing>().Get(m => m.Id == id);
        }

        private ScheduleEmail GetSchedulerEmail(int id)
        {
            return unitWork.Repository<ScheduleEmail>().Get(m => m.ID == id);
        }

        private List<DeliveryCompany> GetDeliveryCompanies()
        {
            return unitWork.Repository<DeliveryCompany>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private List<ConfigMenu> GetConfigMenus()
        {
            return unitWork.Repository<ConfigMenu>().GetAll(m => m.CompanyId == cId).ToList();
        }

        private ConfigMenu GetConfigMenu(int id)
        {
            return unitWork.Repository<ConfigMenu>().Get(m => m.MenuId == id);
        }

        private ConfigChildMenu GetConfigChildMenu(int id)
        {
            return unitWork.Repository<ConfigChildMenu>().Get(m => m.Id == id);
        }

        private void SetResponseStatus(Exception e)
        {
            rs.Status = SBSConstants.Failed;
            rs.Message = e.Message;
        }

        private void SetStatusPageExist()
        {
            rs.Status = SBSConstants.Failed;
            rs.Message = SBSMessages.PageExists;
        }

        private void CheckErrorStates()
        {
            if (rs.ErrorStates == null)
            {
                rs.ErrorStates = new List<ErrorState>();
            }
        }
    }
}