using System;
using System.Collections.Generic;
using System.Globalization;
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
        
        public static string ConvertMoneyString(double money)
        {
            if (money==0)
            {
                return "0";
            }
            return string.Format("{0:0.##}", Math.Round(money, 2));
        }
        public static double ConvertMoneyDouble(double money)
        {
            return Math.Round(money, 2);
        }

        public static string ToMonthName(this DateTime datetime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(datetime.Month);
        }

        public static string ToShortMonthName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(dateTime.Month);
        }

        public static string ToShortDayName(this DateTime dateTime)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedDayName(dateTime.DayOfWeek);
        }
    }
}