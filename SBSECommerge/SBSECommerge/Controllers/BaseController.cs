using SBSECommerge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace SBSECommerge.Controllers
{
    public class BaseController : Controller
    {

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
                return new AppUser(this.User as ClaimsPrincipal);
            }
        }

        public int GetIdUser()
        {
            var thisUser = CurrentUser;
            SBS_DevEntities db = new SBS_DevEntities();
            var user= db.Users.Where(m => m.Email == thisUser.Identity.Name).FirstOrDefault();
            return user.Id;
        }
    }
}