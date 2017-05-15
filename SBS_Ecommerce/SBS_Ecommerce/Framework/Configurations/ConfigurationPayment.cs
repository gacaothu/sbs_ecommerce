using PayPal.Api;
using SBS_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Framework.Configuration
{
    public static class ConfigurationPayment
    {


        // Static constructor for setting the readonly static members.
        static ConfigurationPayment()
        {

        }

        // Create the configuration map that contains mode and other optional configuration details.
        public static Dictionary<string, string> GetConfig()
        {
            SBS_Entities db = new SBS_Entities();
            var configPaypal = db.ConfigPaypals.FirstOrDefault();
            //var config= PayPal.Api.ConfigManager.Instance.GetProperties();
            var config = new Dictionary<string, string>();
            config.Add("mode", configPaypal.Mode);
            config.Add("connectionTimeout", configPaypal.ConnectionTimeout.ToString());
            config.Add("requestRetries", "1");
            config.Add("clientId", configPaypal.ClientId);
            config.Add("clientSecret", configPaypal.ClientSecret);
            return config;
        }

        // Create accessToken
        private static string GetAccessToken()
        {
            var config = GetConfig();
            string ClientId = config["clientId"];
            string ClientSecret = config["clientSecret"];
            // ###AccessToken
            // Retrieve the access token from
            // OAuthTokenCredential by passing in
            // ClientID and ClientSecret
            // It is not mandatory to generate Access Token on a per call basis.
            // Typically the access token can be generated once and
            // reused within the expiry window                
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        // Returns APIContext object
        public static APIContext GetAPIContext()
        {
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            // (that ensures idempotency). The SDK generates
            // a request id if you do not pass one explicitly. 
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();

            // Use this variant if you want to pass in a request id  
            // that is meaningful in your application, ideally 
            // a order id.
            // String requestId = Long.toString(System.nanoTime();
            // APIContext apiContext = new APIContext(GetAccessToken(), requestId ));

            return apiContext;
        }

    }
}