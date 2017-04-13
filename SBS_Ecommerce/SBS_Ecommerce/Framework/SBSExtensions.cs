using System;
using System.Collections.Generic;
using System.Linq;

namespace SBS_Ecommerce.Framework
{
    public static class SBSExtensions
    {
        /// <summary>
        /// Determines whether [is null or empty].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified enumerable]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return enumerable.Any();
        }
        /// <summary>
        /// Get unique string
        /// </summary>
        /// <returns></returns>
        public static string GetNameUnique()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssfff-");
        }
        /// <summary>
        /// Get unique idOrder
        /// </summary>
        /// <returns></returns>
        public static string GetIdOrderUnique()
        {
            return DateTime.Now.ToString("OR-yyyyMMddHHmmssfff");
        }
    }
}