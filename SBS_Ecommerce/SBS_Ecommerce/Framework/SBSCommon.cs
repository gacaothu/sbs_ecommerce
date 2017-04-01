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
        private List<Product> lstTempSearchProducts;
        private List<Product> lstTempProductsCategory;
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
        /// Sets the temporary search products.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SetTempSearchProducts(List<Product> data)
        {
            lstTempSearchProducts?.Clear();
            lstTempSearchProducts = data;
        }

        /// <summary>
        /// Gets the temporary search products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetTempSearchProducts()
        {
            return lstTempSearchProducts;
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

        /// <summary>
        /// Gets the temporary product by category.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetTempProductByCategory()
        {
            return lstTempProductsCategory;
        }

        /// <summary>
        /// Sets the temporary product by category.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SetTempProductByCategory(List<Product> data)
        {
            lstTempProductsCategory?.Clear();
            lstTempProductsCategory = data;
        }

        private SBSCommon()
        {
            
        }
    }
}