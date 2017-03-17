using SBSECommerge.Framework.Configurations;
using System.Diagnostics;

namespace SBSECommerge.Framework.Utilities
{
    public sealed class LoggingUtil
    {
        public static void StartLog(string className, string methodName)
        {
            Debug.WriteLine(className + SBSConstants.Empty + methodName + " has started.");
        }

        public static void EndLog(string className, string methodName)
        {
            Debug.WriteLine(className + SBSConstants.Empty + methodName + " has ended.");
        }
    }
}