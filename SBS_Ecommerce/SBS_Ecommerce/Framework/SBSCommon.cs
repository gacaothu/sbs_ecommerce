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
        private static volatile SBSCommon instance;
        private static object syncRoot = new object();
        private List<Category> lstCategory;
        private List<Product> lstProducts;
        private List<PriceRange> lstPriceRange;
        private List<Brand> lstBrand;
        private List<Bank> lstBank;
        private List<BankAcount> lstBankAccount;
        private List<string> lstTags;

        private Company company;

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
            if (HttpContext.Current.Session[SBSConstants.SessionCategory + cId] != null)
            {
                return (List<Category>)HttpContext.Current.Session[SBSConstants.SessionCategory + cId];
            }
            lstCategory = new List<Category>();
            try
            {
                int plength = 50;
                int pno = 1;
                string sort = "desc";
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListCategory, plength, pno, sort, cId));
                var json = JsonConvert.DeserializeObject<CategoryDTO>(value);
                lstCategory = json.Items;
                foreach (var item in lstCategory)
                {
                    value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListChildCategory, item.Category_ID, plength, pno, sort, cId));
                    json = JsonConvert.DeserializeObject<CategoryDTO>(value);
                    item.Items = json.Items;
                }
                HttpContext.Current.Session[SBSConstants.SessionCategory + cId] = lstCategory;
            }
            catch (Exception e)
            {
            }
            return lstCategory;
        }

        public Product GetProduct(int id)
        {
            string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetProduct, id));
            ProductDetailDTO result = new ProductDetailDTO();
            try
            {
                result = JsonConvert.DeserializeObject<ProductDetailDTO>(value);
            }
            catch (Exception e)
            {
            }
            return result.Items;
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetProducts()
        {
            if (HttpContext.Current.Session[SBSConstants.SessionProduct + cId] != null)
            {
                return (List<Product>)HttpContext.Current.Session[SBSConstants.SessionProduct + cId];
            }
            lstProducts = new List<Product>();

            try
            {
                int pNo = 1;
                int pLength = 1000;
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProduct, cId, pNo, pLength));
                var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                lstProducts = json.Items;
                HttpContext.Current.Session[SBSConstants.SessionProduct + cId] = lstProducts;
            }
            catch (Exception e)
            {
            }
            return lstProducts;
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <returns></returns>
        public List<Product> GetSearchProducts(string text)
        {
            lstProducts = new List<Product>();
            try
            {
                int pNo = 1;
                int pLength = 5;
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListSearchProduct, cId, pNo, pLength, text));
                var json = JsonConvert.DeserializeObject<ProductListDTO>(value);
                lstProducts = json.Items;
            }
            catch (Exception e)
            {
            }
            return lstProducts;
        }

        /// <summary>
        /// Get list promotion
        /// </summary>
        /// <returns>List promotion</returns>
        public List<Product> GetListPromotion()
        {
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
            }
            return lstProducts;
        }

        public List<Product> GetListArrivals()
        {
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
            }
            return lstProducts;
        }

        public List<Product> GetListBestSeller()
        {
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
            }
            return lstProducts;
        }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <returns></returns>
        public List<string> GetTags()
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
            }
            return lstTags;
        }

        /// <summary>
        /// Gets list product review.
        /// </summary>
        /// <returns></returns>
        public List<ProductReview> GetLstProductReview(int pID)
        {
            List<ProductReview> lstProductReview = new List<ProductReview>();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListProductReview, pID));
                var json = JsonConvert.DeserializeObject<LstProductReviewDTO>(value);
                lstProductReview = json.Items.Where(m => m.Record_Status == "Active").ToList();
            }
            catch (Exception e)
            {
            }
            return lstProductReview;
        }

        /// <summary>
        /// Gets the company.
        /// </summary>
        /// <returns></returns>
        public Company GetCompany()
        {
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["Company"] != null)
            {
                return (Company)HttpContext.Current.Session["Company"];
            }
            else
            {
                string domain = "";
                var host = HttpContext.Current.Request.Url.AbsoluteUri;
                //var host = "http://bc.pozento.com";
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
                    //domain = "bc";
                    string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetCompany, domain));
                    var json = JsonConvert.DeserializeObject<CompanyDTO>(value);
                    company = json.Items;
                }
                catch (Exception e)
                {
                }

                return company;
            }
        }

        /// <summary>
        /// Gets the price range.
        /// </summary>
        /// <returns></returns>
        public List<PriceRange> GetPriceRange()
        {
            if (HttpContext.Current.Session[SBSConstants.SessionPriceRange + cId] != null)
            {
                return (List<PriceRange>)HttpContext.Current.Session[SBSConstants.SessionPriceRange + cId];
            }

            lstPriceRange = new List<PriceRange>();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetPriceRange, cId));
                var json = JsonConvert.DeserializeObject<PriceRangeDTO>(value);
                lstPriceRange = json.Items;
                HttpContext.Current.Session[SBSConstants.SessionPriceRange + cId] = lstPriceRange;
            }
            catch (Exception e)
            {
            }

            return lstPriceRange;
        }

        /// <summary>
        /// Gets the brands.
        /// </summary>
        /// <returns></returns>
        public List<Brand> GetBrands()
        {
            if (HttpContext.Current.Session[SBSConstants.SessionBrand + cId] != null)
            {
                return (List<Brand>)HttpContext.Current.Session[SBSConstants.SessionBrand + cId];
            }
            lstBrand = new List<Brand>();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetBrand, cId));
                var json = JsonConvert.DeserializeObject<BrandDTO>(value);
                lstBrand = json.Items;
                HttpContext.Current.Session[SBSConstants.SessionBrand + cId] = lstBrand;
            }
            catch (Exception e)
            {
            }

            return lstBrand;
        }

        /// <summary>
        /// Gets the bank.
        /// </summary>
        /// <returns></returns>
        public List<Bank> GetListBank(int ctryID)
        {
            string methodName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListBank, ctryID));
                var json = JsonConvert.DeserializeObject<BankDTO>(value);
                lstBank = json.Items;
            }
            catch (Exception e)
            {
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

            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.GetListBankAcount, ctryID));
                var json = JsonConvert.DeserializeObject<BankAcountDTO>(value);
                lstBankAccount = json.Items;
            }
            catch (Exception e)
            {
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
            rates rates = new rates();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.LINK_API_CONVERT_MONNEY, currency));
                var json = JsonConvert.DeserializeObject<MoneyToUSD>(value);
                rates = json.rates;
            }
            catch (Exception e)
            {
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
            TaxProduct tax = new TaxProduct();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.LINK_API_GET_TAX, cId));
                var json = JsonConvert.DeserializeObject<TaxProductDTO>(value);
                tax = json.Items;
            }
            catch (Exception e)
            {
            }
            if (tax == null)
            {
                return 0;
            }
            return tax.Tax_Percen;
        }
        /// <summary>
        /// Get Promotion Coupon Of Product
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pIds"></param>
        /// <returns></returns>
        public List<PromotionCoupon> GetPromotionCouponOfProduct(string code, string pIds)
        {
            List<PromotionCoupon> lstPromotionCoupon = new List<PromotionCoupon>();
            try
            {
                string value = RequestUtil.SendRequest(string.Format(SBSConstants.LINK_API_GET_PROMOTION, cId, code, pIds));
                var json = JsonConvert.DeserializeObject<PromotionCouponDTO>(value);
                lstPromotionCoupon = json.Items;
            }
            catch (Exception e)
            {
            }
            return lstPromotionCoupon;
        }
    }
}