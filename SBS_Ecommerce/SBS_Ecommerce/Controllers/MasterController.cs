using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    [MasterAuthorize(Roles = "Admin")]
    public class MasterController : Controller
    {
        private SBSUnitWork unitWork;

        public MasterController()
        {
            unitWork = new SBSUnitWork();
        }

        // GET: Master
        public ActionResult UploadTheme()
        {
            var cId = unitWork.Repository<Models.Theme>().FirdRecord().CompanyId;
            ViewBag.Themes = unitWork.Repository<Models.Theme>().GetAll(m => m.CompanyId == cId).ToList();
            ViewBag.Title = "Upload Theme";
            return View();
        }

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file, string description)
        {
            ResponseResult rs = new ResponseResult();
            string extractPath = "";
            try
            {
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string name = file.FileName.Replace(".zip", "");
                string mimeType = file.ContentType;
                Stream fileContent = file.InputStream;

                var checkExist = unitWork.Repository<Models.Theme>().Get(m => m.Name == name);
                if (checkExist != null)
                {
                    rs.Status = SBSConstants.Exists;
                    rs.Message = SBSMessages.ThemeExist;
                }
                else
                {
                    extractPath = Server.MapPath("~/") + "/Content/Upload/" + name;
                    if (!Directory.Exists(extractPath))
                    {
                        Directory.CreateDirectory(extractPath);
                    }
                    string pathSave = extractPath + "/" + fileName;
                    file.SaveAs(pathSave);

                    //Extra zip file
                    string zipPath = pathSave;

                    ZipFile.ExtractToDirectory(zipPath, extractPath);

                    Helper helper = new Helper();
                    
                    //Copy folder to Content
                    helper.DirectoryCopy(extractPath + "/" + name + "/Content", Server.MapPath("~/") + "/Content/Theme/" + name, true);

                    //Copy folder to Views
                    helper.DirectoryCopy(extractPath + "/" + name + "/Views", Server.MapPath("~/") + "/Views/Theme/" + name, true);

                    Directory.Delete(extractPath, true);

                    //Save to database
                    var lstCid = unitWork.Repository<Models.Theme>().GetAll().Select(m => m.CompanyId).Distinct().ToList();
                    string pathView = "~/Views/Theme/" + name;
                    string pathContent = "~/Content/Theme/" + name;
                    string pathThumb = pathContent + "/img/thumbTheme.png";
                    foreach (var item in lstCid)
                    {
                        Models.Theme theme = new Models.Theme();
                        theme.Name = name;
                        theme.PathView = pathView;
                        theme.PathContent = pathContent;
                        theme.Thumb = pathThumb;
                        theme.Active = false;
                        theme.Description = description;
                        theme.CompanyId = item;
                        unitWork.Repository<Models.Theme>().Add(theme);
                    }
                    unitWork.SaveChanges();

                    rs.Status = SBSConstants.Success;
                    rs.Message = SBSMessages.ThemeSuccess;
                }
            }
            catch (Exception e)
            {
                Directory.Delete(extractPath, true);
                rs.Status = SBSConstants.Failed;
                rs.Message = e.Message;
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

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
                    return RedirectToAction("UploadTheme");
                }
            }

            ViewBag.MessageError = "User name or Password is incorrect.";
            return View(adminLoginDTO);
        }
    }
}