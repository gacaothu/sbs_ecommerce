using SBS_Ecommerce.Framework.Repositories;
using SBS_Ecommerce.Models;
using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class BaseUtil
    {
        private Company company = new Company();
        private int companyID = SBSCommon.Instance.GetCompany().Company_ID;
        private static volatile BaseUtil instance;
        private SBSUnitWork unitWork;

        public BaseUtil()
        {
            unitWork = new SBSUnitWork();
        }

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
            var theme = GetThemeActive();
            return theme.PathView;
        }

        public string GetLayout()
        {
            var theme = GetThemeActive();
            return theme.PathView + "/_Layout.cshtml";
        }

        public string GetPathContent()
        {
            var theme = GetThemeActive();
            return theme.PathContent;
        }

        public List<ConfigSlider> GetSliders()
        {
            return unitWork.Repository<ConfigSlider>().GetAll(m => m.CompanyId == companyID).ToList();
        }

        public string GetNameByEmail(string idUser)
        {
            if (string.IsNullOrEmpty(idUser))
            {
                return null;
            }
            var userLogin = unitWork.Repository<AspNetUser>().Find(idUser);
            if (userLogin == null)
            {
                return null;
            }
            var user = unitWork.Repository<User>().Get(u => u.Email == userLogin.Email && u.CompanyId == companyID);
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
            return unitWork.Repository<ConfigMenu>().GetAll(m => m.CompanyId == companyID).OrderBy(m => m.Position).ToList();
        }

        public Theme GetThemeActive()
        {
            return unitWork.Repository<Theme>().Get(m => m.CompanyId == companyID && m.Active);
        }
    }
}