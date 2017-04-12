using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.Base;
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

        public string GetLayout()
        {
            Helper helper = new Helper();
            List<Models.Base.Theme> themes = new List<Models.Base.Theme>();
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

        public void UpdateUser(string userName)
        {
            var CP = ClaimsPrincipal.Current.Identities.First();
            var AccountNo = CP.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData).Value;
            CP.RemoveClaim(new Claim(ClaimTypes.UserData, AccountNo));
            CP.AddClaim(new Claim(ClaimTypes.UserData, userName));
        }
        public int GetIdUserCurrent()
        {
            var user =  UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                return -1;
            }
            var nameUser = user.Email;
            using (var db = new SBS_Entities()) {
                var userDb = db.Users.Where(m => m.Email == nameUser).FirstOrDefault();
                return userDb != null ? userDb.Id : -1;
            } 
        }
       
    }
}