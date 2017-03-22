namespace SBSECommerge.Framework.Configurations
{
    public sealed class SBSConstants
    {
        public const string Empty = " ";
        public const string Email = "Email";
        public const string Code = "Code";
        public const string Password = "Password";
        public const string ConfirmPassword = "Confirm password";
        public const string RememberMe = "Remember me?";
        public const string RememberBrowser = "Remember browser?";

        // Staging
        public const string Domain = "http://qa.bluecube.com.sg/pos3v2-wserv";
        public const string GetListProduct = Domain + "/wserv/LstProduct?plength=10&pno=1&sort=desc&cID=1";
    }
}