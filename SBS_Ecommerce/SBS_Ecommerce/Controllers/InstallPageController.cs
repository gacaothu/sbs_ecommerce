using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class InstallPageController : Controller
    {
        private const string PathContent = "~/Content/Theme/";
        private const string PathViews = "~/Views/Theme/";
        private const string PathViewDefaultTheme = "~/Views/Theme/Default";
        private const string PathContentDefaultTheme = "~/Content/Theme/Default";
        private const string PathDefaultThumbTheme = "~/Content/Theme/Default/img/thumbTheme.png";

        Helper helper = new Helper();
        Models.SBS_Entities db = new Models.SBS_Entities();

        // GET: InstallPage
        public ActionResult Index(int cpID)
        {
            //var theme = db.Themes.Where(m => m.CompanyId == cpID).FirstOrDefault();
            var theme = db.GetThemes.FirstOrDefault();
            if (theme == null)
            {
                ViewBag.CompanyID = cpID;
                return View(cpID);
            }
            else
            {
                return Redirect(Url.Action("Index", "Home"));
            }
        }

        [HttpPost]
        public ActionResult Install(int cpID)
        {
            //var theme = db.Themes.Where(m => m.CompanyId == cpID).FirstOrDefault();
            //if (theme == null)
            //{
            //    //1. Create folder with companyID on theme and content
            //    //Check exist folder cpid on content
            //    var pathServerContent = Server.MapPath(pathContent + cpID.ToString() + "/Default");
            //    if (!Directory.Exists(pathServerContent))
            //    {
            //        Directory.CreateDirectory(pathServerContent);
            //    }
            //    else
            //    {
            //        //If exist delete it and create new default
            //        Directory.Delete(pathServerContent, true);
            //        Directory.CreateDirectory(pathServerContent);
            //    }

            //    //Copy Default content to Content
            //    helper.DirectoryCopy(Server.MapPath(pathContentDefaultTheme), pathServerContent, true);

            //    //2. Create folder with companyID on theme and view
            //    //Check exist folder cpid on view
            //    var pathServerViews = Server.MapPath(pathViews + cpID.ToString() + "/Default");
            //    if (!Directory.Exists(pathServerViews))
            //    {
            //        Directory.CreateDirectory(pathServerViews);
            //    }
            //    else
            //    {
            //        //If exist delete it and create new default
            //        Directory.Delete(pathServerViews, true);
            //        Directory.CreateDirectory(pathServerViews);
            //    }

            //    //Copy Default views to Views
            //    helper.DirectoryCopy(Server.MapPath(pathViewDefaultTheme), pathServerViews, true);
            //    //Save to database
            //    Models.Theme newTheme = new Models.Theme();
            //    newTheme.CompanyId = cpID;
            //    newTheme.Name = "Default";
            //    newTheme.Path = "~/Views/Theme/" + cpID.ToString() + "/Default";
            //    newTheme.Active = true;
            //    db.Themes.Add(newTheme);
            //    db.SaveChanges();

            //    return Json(true);

            //}
            //else
            //{
            //    return Json(false);
            //}

            // new source
            var theme = db.GetThemes.FirstOrDefault();
            if (theme == null)
            {
                // New Theme record
                Models.Theme newTheme = new Models.Theme();
                newTheme.CompanyId = cpID;
                newTheme.Name = "Default";
                newTheme.PathView = PathViewDefaultTheme;
                newTheme.PathContent = PathContentDefaultTheme;
                newTheme.Thumb = PathDefaultThumbTheme;
                newTheme.Active = true;
                db.Themes.Add(newTheme);

                // Add Default Slider
                List<ConfigSlider> lstSliders = new List<ConfigSlider>();
                for (int i = 0; i < 5; i++)
                {
                    ConfigSlider slider = new ConfigSlider()
                    {
                        CompanyId = cpID,
                        Name = "Picture " + (i + 1),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    lstSliders.Add(slider);
                }
                db.ConfigSliders.AddRange(lstSliders);
                db.SaveChanges();

                Task.Delay(1500);
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}