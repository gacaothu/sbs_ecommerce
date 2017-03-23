namespace SBS_Ecommerce.Framework.Configurations
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

        // API
        public const string Domain = "http://qa.bluecube.com.sg/pos3v2-wserv";
        public const string GetListProduct = Domain + "/wserv/LstProduct?plength=10&pno=1&sort=desc&cID=1";
        public const string GetListCategory = Domain + "/wserv/LstCategory?plength=2&pno=1&sort=desc&cID=1";

        public const int Success = 0;
        public const int Failed = -1;
        public const int Exists = 1;

        public const string Active = "1";
        public const string InActive = "0";
    }
}