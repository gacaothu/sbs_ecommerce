using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.IO.Compression;
using SBS_Ecommerce.Models.Base;

namespace SBS_Ecommerce.Controllers
{
    public class AdminController : BaseController
    {
        List<Theme> themes = new List<Theme>();
        Helper helper = new Helper();
        // GET: Admin
        public ActionResult Index()
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            ViewBag.Themes = themes;

            return RedirectToAction("LayoutManager");
        }

        public ActionResult ThemeManager()
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            ViewBag.Themes = themes;
            ViewBag.Title = "Theme Manager";
            return View();
        }

        public ActionResult ChangeLayout(List<int> lstID)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");

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

            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml", lstLayoutNew);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LayoutManager()
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayout = new List<Layout>();
            lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            //Session["RenderLayout"] = lstLayout;
            ViewBag.RenderLayout = lstLayout;
            Slider slider = new Slider();
            slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configslider.xml");
            ViewBag.RenderSlider = slider;
            ViewBag.Title = "Layout Manager";
            return View();
        }

        public ActionResult ActiveBlock(int id)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = true;
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml", lstLayout);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeactiveBlock(int id)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            lstLayout.Where(m => m.ID == id).FirstOrDefault().Active = false;
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml", lstLayout);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetHTML(int id)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();
            return Json(new { Title = layout.Name, Content = layout.Content }, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult AddHTML(string content, string title)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
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

            layout.Path = "~\\Views\\Theme\\Theme3\\_PartialHTML.cshtml";
            layout.Content = content;
            layout.Active = true;
            layout.CanEdit = true;
            lstLayout.Add(layout);
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml", lstLayout);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult EditHTML(string content, string title, int id)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            var layout = lstLayout.Where(m => m.ID == id).FirstOrDefault();
            layout.Content = content;
            layout.Name = title;
            helper.SerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml", lstLayout);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview(string id)
        {
            string[] lstID = id.Split('_');
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Layout> lstLayoutNew = new List<Layout>();
            Session["Layout"] = themes.Where(m => m.Active).FirstOrDefault().Path;

            var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");

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
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");

            //Get category from API
            ViewBag.LstCategory = helper.GetCategory();

            ViewBag.RenderMenu = lstMenu.ToList();
            ViewBag.RenderLayout = lstLayoutNew;
            return View();
        }

        public ActionResult PreViewMenu(string id)
        {
            string[] lstID = id.Split('_');
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenuNew = new List<Menu>();
            Session["Layout"] = themes.Where(m => m.Active).FirstOrDefault().Path;

            //var lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");

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
            try
            {
                lstLayout = helper.DeSerializeLayout(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/layout.xml");
            }
            catch
            {

            }

            //Get category from API
            ViewBag.LstCategory = helper.GetCategory();
            ViewBag.RenderMenu = lstMenuNew;
            ViewBag.RenderLayout = lstLayout.Where(m => m.Active).ToList();
            return View();
        }

        public ActionResult AddGadgate()
        {
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeActive(int id)
        {
            try
            {
                themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
                var theme = themes.Where(m => m.ID == id).FirstOrDefault();

                //Set all theme to false
                themes.ForEach(m => m.Active = false);

                //Set theme to true
                theme.Active = true;

                //Save XML
                helper.Serialize(Server.MapPath("~") + "/Content/theme.xml", themes);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

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
                string pathSave = Server.MapPath("~/") + "/Views/Theme/ExtraTheme/" + fileName;
                file.SaveAs(pathSave); //File will be saved in application root

                //Extra zip file
                string zipPath = pathSave;
                string extractPath = Server.MapPath("~/") + "/Views/Theme/ExtraTheme/";
                ZipFile.ExtractToDirectory(zipPath, extractPath);

                //Copy folder to Content
                helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Content", Server.MapPath("~/Content/Theme/") + fileName.Replace(".zip", ""), true);

                //Copy folder to Views
                helper.DirectoryCopy(extractPath + fileName.Replace(".zip", "") + "/Views", Server.MapPath("~/Views/Theme/") + fileName.Replace(".zip", ""), true);

                //Save to database
                themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
                Theme theme = new Theme();
                theme.ID = themes.LastOrDefault().ID + 1;
                theme.Name = fileName.Replace(".zip", "");
                theme.Path = "~/Views/Theme/" + fileName.Replace(".zip", "");
                themes.Add(theme);
                helper.Serialize(Server.MapPath("~") + "/Content/theme.xml", themes);

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
                themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
                var theme = themes.Where(m => m.ID == id).FirstOrDefault();

                string extractDelete = Server.MapPath("~/") + "/Views/Theme/ExtraTheme/" + theme.Name;

                //Delete folder on extra
                Directory.Delete(extractDelete, true);

                //Delete file .zip
                System.IO.File.Delete(extractDelete + ".zip");

                //Delete on view
                Directory.Delete(Server.MapPath("~/") + "/Views/Theme/" + theme.Name, true);

                //Delete on content
                Directory.Delete(Server.MapPath("~/") + "/Content/Theme/" + theme.Name, true);

                //If theme on active then set themedefault active
                if (theme.Active)
                {
                    themes.Where(m => m.ID == 1).FirstOrDefault().Active = true;
                }

                //Remove theme in database
                themes.Remove(theme);
                helper.Serialize(Server.MapPath("~") + "/Content/theme.xml", themes);

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SaveConfigSlider()
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            Slider slider = new Slider();
            slider = helper.DeSerializeSlider(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configslider.xml");

            //Get path of theme active
            var theme = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml").Where(m => m.Active).FirstOrDefault();

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
                var pathContentofTheme = Server.MapPath("~/") + "/Content/Theme/" + theme.Name;

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

                picture.Path = "/Content/Theme/" + theme.Name + "/img/slider/" + random + fileName;
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
            helper.SerializeSlider(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configslider.xml", slider);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        private string RandomString()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff");
        }

        public ActionResult MenuManager()
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> menu = new List<Menu>();
            menu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
            ViewBag.Title = "Menu Manager";
            ViewBag.LstMenu = menu;
            return View();
        }

        [ValidateInput(false)]
        public ActionResult AddMenu(string name, string url)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");

            //Create menu
            Menu menu = new Menu();
            menu.ID = lstMenu.OrderBy(m => m.ID).LastOrDefault().ID + 1;
            menu.Name = name;
            menu.Href = url;

            //Save to xml configmenu
            lstMenu.Add(menu);
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult EditMenu(int id, string name, string url)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");

            var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            menu.Name = name;
            menu.Href = url;

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);

            //Return status
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMenu(int id)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == id).FirstOrDefault();
            lstMenu.Remove(menu);

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult AddChildMenu(int id, string name, string url)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
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
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult EditChildMenu(int parentID, int childrenID, string name, string url)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            //Get childmenu
            var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            childMenu.Name = name;
            childMenu.Href = url;

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteChildMenu(int parentID, int childrenID)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");
            var menu = lstMenu.Where(m => m.ID == parentID).FirstOrDefault();

            //Get childmenu
            var childMenu = menu.LstChildMenu.Where(m => m.ID == childrenID).FirstOrDefault();
            menu.LstChildMenu.Remove(childMenu);

            //Save to xml configmenu
            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenu);

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveMenu(List<int> lstID)
        {
            themes = helper.DeSerialize(Server.MapPath("~") + "/Content/theme.xml");
            List<Menu> lstMenu = new List<Menu>();
            lstMenu = helper.DeSerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml");

            List<Menu> lstMenuNew = new List<Menu>();
            foreach (var itemID in lstID)
            {
                foreach (var itemLayout in lstMenu)
                {
                    if (itemID == itemLayout.ID)
                    {
                        lstMenuNew.Add(new Menu { ID = itemLayout.ID, Name = itemLayout.Name, Href = itemLayout.Href, LstChildMenu = itemLayout.LstChildMenu });
                    }
                }
            }

            helper.SerializeMenu(Server.MapPath("~") + "/Views/Theme/" + themes.Where(m => m.Active).FirstOrDefault().Name + "/configmenu.xml", lstMenuNew);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}