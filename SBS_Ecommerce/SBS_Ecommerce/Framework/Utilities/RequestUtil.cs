using System.Net;

namespace SBS_Ecommerce.Framework.Utilities
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