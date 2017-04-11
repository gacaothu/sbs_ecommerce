using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models;
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
        private List<string> lstTags;
        private Company company;

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
                    foreach (var item in lstCategory)
                    {
                        value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListChildCategory, item.Category_ID));
                        json = JsonConvert.DeserializeObject<CategoryDTO>(value);
                        item.Items = json.Items;
                    }
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
                    int pLength = 1000;
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

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <returns></returns>
        public List<string> GetTags()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            if (lstTags.IsNullOrEmpty())
            {
                lstTags = new List<string>();
                try
                {
                    int cId = 1;
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetTags, cId));
                    var json = JsonConvert.DeserializeObject<TagDTO>(value);
                    foreach (var item in json.Items)
                    {
                        lstTags.Add(item.Tag_Name);
                    }
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            LoggingUtil.EndLog(ClassName, methodName);
            return lstTags;
        }
        
        
        public Company GetCompany(int cID)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            if (company == null)
            {
                // Khoi tao doi tuong
                company = new Company();
                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, cID));
                    var json = JsonConvert.DeserializeObject<CompanyDTO>(value);
                    company = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            LoggingUtil.EndLog(ClassName, methodName);
            return company;
        }

        private SBSCommon()
        {
        }
    }
}