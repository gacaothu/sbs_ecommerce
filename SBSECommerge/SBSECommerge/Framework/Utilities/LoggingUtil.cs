using SBSECommerge.Framework.Configurations;
using System.Diagnostics;

namespace SBSECommerge.Framework.Utilities
{
    public sealed class LoggingUtil
    {
        /// <summary>
        /// Starts the log.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        public static void StartLog(string className, string methodName)
        {
            Debug.WriteLine(className + SBSConstants.Empty + methodName + " has started.");
        }

        /// <summary>
        /// Ends the log.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        public static void EndLog(string className, string methodName)
        {
            Debug.WriteLine(className + SBSConstants.Empty + methodName + " has ended.");
        }
    }
}