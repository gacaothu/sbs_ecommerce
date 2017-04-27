using Newtonsoft.Json;
using SBS_Ecommerce.Models.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class CountryUtil
    {
        private static volatile CountryUtil instance;
        private static object syncRoot = new object();

        private List<Country> lstCountries;

        private CountryUtil() { }

        public static CountryUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CountryUtil();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Gets the countries.
        /// </summary>
        /// <returns></returns>
        public List<Country> GetCountries()
        {
            if (lstCountries.IsNullOrEmpty())
            {
                var content = File.ReadAllText(HostingEnvironment.MapPath(@"~/countries.txt"));
                lstCountries = JsonConvert.DeserializeObject<List<Country>>(content);
            }
            return lstCountries;
        }
    }
}