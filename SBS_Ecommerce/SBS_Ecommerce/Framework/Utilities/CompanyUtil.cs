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
        /// <summary>
        /// Gets the company.
        /// </summary>
        /// <returns></returns>
        public Company GetCompany()
        {
            string domain = "";
            var host = HttpContext.Current.Request.Url.AbsoluteUri;
            string urlNonHttp = host.Substring(host.IndexOf("//") + 2);
            string[] lsSub = urlNonHttp.Split('/');
            if (lsSub != null && lsSub.Count() > 0)
            {

                int indexofSub = lsSub[0].IndexOf(".");
                if (indexofSub > 0)
                {
                    domain = lsSub[0].Substring(0, indexofSub);
                }
                else
                {
                    domain = lsSub[0];
                }
            }

            company = new Company();


            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, domain));
            var json = JsonConvert.DeserializeObject<CompanyDTO>(value);
            company = json.Items;
            return company;
        }

        public string GetThemeName()
        {
            var cpID = GetCompany().Company_ID;
            var theme = db.Themes.Where(m => m.CompanyId == cpID && m.Active).FirstOrDefault();
            return theme.Name;
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