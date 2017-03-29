using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Framework
{
    public sealed class SBSCommon
    {
        private const string ClassName = nameof(SBSCommon);
        private static volatile SBSCommon instance;
        private static object syncRoot = new object();

        private List<Category> lstCategory;

        public static SBSCommon Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SBSCommon();
                    }
                }
                return instance;
            }
        }

        public List<Category> GetCategories()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            if (lstCategory.IsNullOrEmpty())
            {
                try
                {
                    string value = RequestUtil.SendRequest(SBSConstants.GetListCategory);
                    var json = JsonConvert.DeserializeObject<CategoryDTO>(value);
                    lstCategory = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }            
            LoggingUtil.EndLog(ClassName, methodName);
            return lstCategory;
        }

        private SBSCommon()
        {
            lstCategory = new List<Category>();
        }
    }
}