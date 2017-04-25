using Newtonsoft.Json;
using SBS_Ecommerce.Framework.Configurations;
using SBS_Ecommerce.Framework.Utilities;
using SBS_Ecommerce.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace SBS_Ecommerce.Framework
{
    public sealed class SBSCommon
    {
        private const string ClassName = nameof(SBSCommon);
        private static volatile SBSCommon instance;
        private static object syncRoot = new object();
        private int companyId;
        private List<Category> lstCategory;
        private List<Product> lstTempSearchProducts;
        private List<Product> lstTempProductsCategory;
        private List<Product> lstProducts;
        private List<PriceRange> lstPriceRange;
        private List<Brand> lstBrand;
        private List<Bank> lstBank;
        private List<BankAcount> lstBankAccount;
        private List<string> lstTags;
        private List<LoginAdmin> lstAdminLogin;
        private List<Country> lstCountries;

        private Company company;
        // private CompanyUtil cpUtil = new CompanyUtil();
        private int cId;
        public static SBSCommon Instance
        {
            get
            {
                instance = new SBSCommon();
                return instance;
            }
        }

        public SBSCommon()
        {
            var company = GetCompany();
            if (company != null)
                cId = GetCompany().Company_ID;
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
                lstCategory = new List<Category>();
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
            lstProducts = new List<Product>();
            try
            {
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

            LoggingUtil.EndLog(ClassName, methodName);
            return lstProducts;
        }

        /// <summary>
        /// Get list promotion
        /// </summary>
        /// <returns>List promotion</returns>
        public List<Product> GetListPromotion()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            lstProducts = new List<Product>();
            try
            {
                string type = "promotion";
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListPromotion, cId, type));
                var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                lstProducts = json.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return lstProducts;
        }

        public List<Product> GetListArrivals()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);
            lstProducts = new List<Product>();
            try
            {
                string type = "newarrival";
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListArrivals, cId, type));
                var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                lstProducts = json.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }

            LoggingUtil.EndLog(ClassName, methodName);
            return lstProducts;
        }

        public List<Product> GetListBestSeller()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            lstProducts = new List<Product>();
            try
            {
                string type = "bestseller";
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetBestSellerProduct, cId, type));
                var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                lstProducts = json.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
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

        /// <summary>
        /// Gets the company.
        /// </summary>
        /// <returns></returns>
        public Company GetCompany()
        {
            if (HttpContext.Current.Session["Company"] != null)
            {
                return (Company)HttpContext.Current.Session["Company"];
            }
            else
            {
                string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
                LoggingUtil.StartLog(ClassName, methodName);
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

                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, domain));
                    var json = JsonConvert.DeserializeObject<CompanyDTO>(value);
                    company = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }

                LoggingUtil.EndLog(ClassName, methodName);
                HttpContext.Current.Session["Company"] = company;
                return company;
            }
        }

        /// <summary>
        /// Gets the price range.
        /// </summary>
        /// <returns></returns>
        public List<PriceRange> GetPriceRange()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            if (lstPriceRange.IsNullOrEmpty())
            {
                lstPriceRange = new List<PriceRange>();
                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetPriceRange, cId));
                    var json = JsonConvert.DeserializeObject<PriceRangeDTO>(value);
                    lstPriceRange = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            LoggingUtil.EndLog(ClassName, methodName);
            return lstPriceRange;
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns></returns>
        public List<Brand> GetBrands()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            LoggingUtil.StartLog(ClassName, methodName);

            if (lstBrand.IsNullOrEmpty())
            {
                lstBrand = new List<Brand>();
                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetBrand, cId));
                    var json = JsonConvert.DeserializeObject<BrandDTO>(value);
                    lstBrand = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            LoggingUtil.EndLog(ClassName, methodName);
            return lstBrand;
        }

        /// <summary>
        /// Gets the bank.
        /// </summary>
        /// <returns></returns>
        public List<Bank> GetListBank(int ctryID)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            if (lstBank.IsNullOrEmpty())
            {
                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListBank, ctryID));
                    var json = JsonConvert.DeserializeObject<BankDTO>(value);
                    lstBank = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            return lstBank;
        }
        /// <summary>
        /// Gets the bank.
        /// </summary>
        /// <returns></returns>
        public List<BankAcount> GetListBankAccount(int ctryID)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            if (lstBankAccount.IsNullOrEmpty())
            {
                try
                {
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListBankAcount, ctryID));
                    var json = JsonConvert.DeserializeObject<BankAcountDTO>(value);
                    lstBankAccount = json.Items;
                }
                catch (Exception e)
                {
                    LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
                }
            }
            return lstBankAccount;
        }
        /// <summary>
        /// Gets the bank.
        /// </summary>
        /// <param name="cID">The bank identifier.</param>
        /// <returns></returns>
        public double GetRateExchange(string currency)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            rates rates = new rates();

            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.LINK_API_CONVERT_MONNEY, currency));
                var json = JsonConvert.DeserializeObject<MoneyToUSD>(value);
                rates = json.rates;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }
            if (rates.SGD == 0)
            {
                rates.USD = 0;
            }
            return rates.USD / rates.SGD;
        }
        /// <summary>
        /// Gets the bank.
        /// </summary>
        /// <param name="cID">The bank identifier.</param>
        /// <returns></returns>
        public double GetTaxOfProduct()
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            TaxProduct tax = new TaxProduct();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.LINK_API_GET_TAX, cId));
                var json = JsonConvert.DeserializeObject<TaxProductDTO>(value);
                tax = json.Items;
            }
            catch (Exception e)
            {
                LoggingUtil.ShowErrorLog(ClassName, methodName, e.Message);
            }
            if (tax == null)
            {
                return 0;
            }
            return tax.Tax_Percen;
        }
        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <returns></returns>
        public List<Country> GetCountries()
        {
            if (lstCountries.IsNullOrEmpty())
            {
                var content = File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/country.txt"));
                lstCountries = JsonConvert.DeserializeObject<List<Country>>(content);
            }
            return lstCountries;
        }

    }
}