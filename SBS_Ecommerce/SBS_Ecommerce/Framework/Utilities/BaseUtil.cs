using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class BaseUtil
    {
        private Company company = new Company();
        SBS_Entities db = new SBS_Entities();
        private int companyID = SBSCommon.Instance.GetCompany().Company_ID;
        private static volatile BaseUtil instance;

        public static BaseUtil Instance
        {
            get
            {
                instance = new BaseUtil();
                return instance;
            }
        }
        public string GetPathTheme()
        {
            //var theme = db.Themes.Where(m => m.CompanyId == companyID && m.Active).FirstOrDefault();
            //string pathTheme = "~/Views/Theme/" + companyID + "/" + theme.Name;

            var theme = db.GetThemes.FirstOrDefault(m=>m.Active);
            string pathTheme = "~/Views/Theme/" + theme.Name;
            return pathTheme;
        }

        public string GetLayout()
        {
            //var theme = db.Themes.Where(m => m.CompanyId == companyID && m.Active).FirstOrDefault();
            //string pathTheme = "~/Views/Theme/" + companyID + "/" + theme.Name;

            var theme = db.GetThemes.FirstOrDefault(m => m.Active);
            string pathTheme = "~/Views/Theme/" + theme.Name;
            return pathTheme + "/_Layout.cshtml";
        }

        public string GetPathContent()
        {
            //var theme = db.Themes.Where(m => m.CompanyId == companyID && m.Active).FirstOrDefault();
            //string pathContent = "~/Content/Theme/" + companyID + "/" + theme.Name;

            var theme = db.GetThemes.FirstOrDefault(m => m.Active);
            string pathContent = "~/Content/Theme/" + theme.Name;
            return pathContent;
        }
        
        public string GetNameByEmail(string idUser)
        {
            if (string.IsNullOrEmpty(idUser))
            {
                return null;
            }
            var userLogin = db.AspNetUsers.Find(idUser);
            if (userLogin==null)
            {
                return null;
            }
            var user = db.GetUsers.Where(u => u.Email == userLogin.Email && u.CompanyId== companyID).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (string.IsNullOrEmpty(user.FirstName) && string.IsNullOrEmpty(user.LastName))
            {
                return user.Email;
            }
            return user.FirstName + " " + user.LastName;
        }

        public List<ConfigMenu> GetMenus()
        {
            return db.GetConfigMenus.OrderBy(m => m.Position).ToList();
        }
    }
}