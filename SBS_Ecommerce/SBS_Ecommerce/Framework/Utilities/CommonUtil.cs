using System;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class CommonUtil
    {
        private const string OrderPrefix = "OR-";
       
        /// <summary>
        /// Generates the order identifier.
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrderId()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            var unixTimestamp = (long)timeSpan.TotalSeconds;
            return OrderPrefix + unixTimestamp;
        }

        /// <summary>
        /// Get unique string
        /// </summary>
        /// <returns></returns>
        public static string GetNameUnique()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            var unixTimestamp = (long)timeSpan.TotalSeconds;
            return unixTimestamp.ToString();
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}