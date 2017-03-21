using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SBSECommerge.Framework.Utilities
{
    public sealed class RequestUtil
    {
        public static string SendRequest(string url)
        {
            string result = "";
            using(var client = new WebClient())
            {
                client.Proxy = null;
                result = client.DownloadString(url);
            }
            return result;
        }
    }
}