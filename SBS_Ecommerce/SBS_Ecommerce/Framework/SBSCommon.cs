using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;

namespace SBS_Ecommerce.Framework
{
    public sealed class SBSCommon
    {
        private const string ClassName = nameof(SBSCommon);
        private static volatile SBSCommon instance;
        private static object syncRoot = new object();

        private List<Category> lstCategory;
        private List<Product> lstTempProducts;
        private List<Product> lstProducts;

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

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Sets the temporary products.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SetTempProducts(List<Product> data)
        {
            lstTempProducts = data;
        }

        /// <summary>
        /// Gets the temporary products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetTempProducts()
        {
            return lstTempProducts;
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            if (lstProducts.IsNullOrEmpty())
            {
                try
                {
                    int cId = 1;
                    int pNo = 1;
                    int pLength = 50;
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
                    var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                    lstProducts = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);

                }
            }
            LoggingUtil.EndLog(ClassName, methodName);
            return lstProducts;
        }

        private SBSCommon()
        {
            lstCategory = new List<Category>();
            lstProducts = new List<Product>();
            lstTempProducts = new List<Product>();
        }
    }
}