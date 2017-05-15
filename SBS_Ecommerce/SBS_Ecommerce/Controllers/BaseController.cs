using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using SBS_Ecommerce.Framework;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBS_Ecommerce.Controllers
{
    public class BaseController : Controller
    {
        private const string Tilde = "~";
        private const string ThemeXmlPath = "/Content/theme.xml";
        private ApplicationUserManager _userManager;
        public SBS_Entities db = new SBS_Entities();
        public int cId = SBSCommon.Instance.GetCompany().Company_ID;

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
            int cpID = SBSCommon.Instance.GetCompany().Company_ID;
            var theme = db.Themes.Where(m => m.Active && m.CompanyId == cpID).FirstOrDefault();
            var pathView = theme.Path;
            return pathView;
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
            using (var db = new SBS_Entities())
            {
                var userDb = db.GetUsers.Where(m => m.Email == nameUser).FirstOrDefault();
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
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);

                return sw.ToString();
            }
        }

    }
}