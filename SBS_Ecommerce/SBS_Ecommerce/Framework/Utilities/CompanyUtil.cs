using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Framework.Utilities
{
    public class CompanyUtil
    {
        private Company company = new Company();
        Models.SBS_Entities db = new Models.SBS_Entities();
        private int companyID = SBSCommon.Instance.GetCompany().Company_ID;
        public string GetPathTheme()
        {
            var theme = db.Themes.Where(m => m.CompanyId == companyID && m.Active).FirstOrDefault();
            string pathTheme = "~/Views/Theme/" + companyID + "/" + theme.Name;
            return pathTheme;
        }

        public string GetNameByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }
           
            var user = db.GetUsers.Where(u => u.Email == email).FirstOrDefault();
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
    }
}