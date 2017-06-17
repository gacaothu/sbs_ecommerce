using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class BaseController : Controller
    {
        private const string Tilde = "~";
        private const string ThemeXmlPath = "/Content/theme.xml";
        private ApplicationUserManager _userManager;
        public int cId = SBSCommon.Instance.GetCompany().Company_ID;
        private SBSUnitWork unitWork;
        protected ResponseResult rs;

        public BaseController()
        {
            unitWork = new SBSUnitWork();
            rs = new ResponseResult();
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

        /// <summary>
        /// Gets the layout.
        /// </summary>
        /// <returns></returns>
        public string GetLayout()
        {
            var theme = unitWork.Repository<Theme>().Get(m => m.CompanyId == cId && m.Active);
            var pathView = theme.PathView;
            return pathView;
        }

        protected Theme GetThemeActive()
        {
            return unitWork.Repository<Theme>().Get(m => m.CompanyId == cId && m.Active);
        }

        /// <summary>
        /// Get error list from ModelState
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static List<string> GetErrorListFromModelState
                                               (ModelStateDictionary modelState)
        {
            var query = from state in modelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            var errorList = query.ToList();
            return errorList;
        }

        public AppUser CurrentUser
        {
            get
            {
                return new AppUser(User as ClaimsPrincipal);
            }
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void UpdateUser(string userName)
        {
            var CP = ClaimsPrincipal.Current.Identities.First();
            var AccountNo = CP.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData).Value;
            CP.RemoveClaim(new Claim(ClaimTypes.UserData, AccountNo));
            CP.AddClaim(new Claim(ClaimTypes.UserData, userName));
        }

        /// <summary>
        /// Gets the identifier user current.
        /// </summary>
        /// <returns></returns>
        public int GetIdUserCurrent()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return -1;
            }
            var nameUser = user.Email;
            var cacheUser = Session[SBSConstants.SessionUser + nameUser] as User;
            if (cacheUser != null)
            {
                return cacheUser.Id;
            }
            else
            {
                var userDb = unitWork.Repository<User>().Get(m => m.Email == nameUser);
                Session[SBSConstants.SessionUser + nameUser] = userDb;
                return userDb != null ? userDb.Id : -1;
            }
        }

        protected string PartialViewToString(Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);

                viewResult.View.Render(viewContext, sw);
                // viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.ToString();
            }
        }

        protected void InitSEO(string scheme, string host, string path)
        {
            SEO seo = null;
            if (!string.IsNullOrEmpty(path) && !path.Contains("Home/Index") && (path.Split('/').Length - 1) > 2)
            {
                seo = unitWork.Repository<SEO>().Get(m => m.Url.Contains(host + path));
            }
            else
            {
                seo = unitWork.Repository<SEO>().Get(m => m.Url == (scheme + "://" + host + path));
            }

            ViewData["Keywords"] = !string.IsNullOrEmpty(seo?.Keywords) ? seo?.Keywords : "";
            ViewData["Description"] = !string.IsNullOrEmpty(seo?.Description) ? seo?.Description : "";
        }

        protected ConfigPaypal GetConfigPaypal()
        {
            return unitWork.Repository<ConfigPaypal>().Get(m => m.CompanyId == cId);
        }

        protected ConfigChatting GetConfigChatting()
        {
            return SBSCommon.Instance.GetConfigChatting();
        }

        protected ConfigMailChimp GetConfigMailChimp()
        {
            return unitWork.Repository<ConfigMailChimp>().Get(m => m.CompanyId == cId);
        }

        protected List<Theme> GetThemes()
        {
            return unitWork.Repository<Theme>().GetAll(m => m.CompanyId == cId).ToList();
        }

        protected Theme GetTheme(int id)
        {
            return unitWork.Repository<Theme>().Get(m => m.ID == id);
        }
    }
}