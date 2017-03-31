﻿using SBS_Ecommerce.Framework.Configurations;
using System.Diagnostics;

namespace SBS_Ecommerce.Framework.Utilities
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
            Debug.WriteLine(className + SBSConstants.Space + methodName + " has started.");
        }

        /// <summary>
        /// Ends the log.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        public static void EndLog(string className, string methodName)
        {
            Debug.WriteLine(className + SBSConstants.Space + methodName + " has ended.");
        }

        /// <summary>
        /// Shows the error log.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="error">The error.</param>
        public static void ShowErrorLog(string className, string methodName, string error)
        {
            Debug.WriteLine(className + SBSConstants.Space + methodName + " has error with message: " + error + " .");
        }
    }
}