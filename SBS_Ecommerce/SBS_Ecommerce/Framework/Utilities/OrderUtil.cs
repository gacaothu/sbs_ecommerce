using System;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class OrderUtil
    {
        private const string OrderPrefix = "OR-";

        /// <summary>
        /// Generates the order identifier.
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrderId()
        {            
            return OrderPrefix + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}