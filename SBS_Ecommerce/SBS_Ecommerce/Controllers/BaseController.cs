using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace SBS_Ecommerce.Controllers
{
    public class BaseController : Controller
    {
        private const string Tilde = "~";
        private const string ThemeXmlPath = "/Content/theme.xml";

        public string GetLayout()
        {
            Helper helper = new Helper();
            List<Theme> themes = new List<Theme>();
            themes = helper.DeSerialize(Server.MapPath(Tilde) + ThemeXmlPath);
            var layOut = themes.Where(m => m.Active == true).FirstOrDefault().Path;
            return layOut;
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

        public int GetIdUserCurrent()
        {
            using (var db = new SBS_DevEntities()) {
                var user = db.Users.Where(m => m.Email == CurrentUser.Identity.Name).FirstOrDefault();
                return user != null ? user.Id : -1;
            } 
        }
    }
}