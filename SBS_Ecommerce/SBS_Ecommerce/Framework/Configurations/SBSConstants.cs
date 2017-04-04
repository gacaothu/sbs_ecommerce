namespace SBS_Ecommerce.Framework.Configurations
{
    public sealed class SBSConstants
    {
        public const string Empty = "";
        public const string Space = " ";
        public const string Email = "Email";
        public const string Code = "Code";
        public const string Password = "Password";
        public const string ConfirmPassword = "Confirm password";
        public const string RememberMe = "Remember me?";
        public const string RememberBrowser = "Remember browser?";

        // API
        public const string Domain = "http://qa.bluecube.com.sg/pos3v2-wserv";
        public const string GetListProduct = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}";
        public const string GetListCategory = Domain + "/wserv/LstCategory?plength=2&pno=1&sort=desc&cID=1";
        public const string GetProduct = Domain + "/WServ/GetProduct?pID={0}";
        public const string GetBestSellerProduct = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&bestseller=true";
        public const string GetListProductByCategory = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&cgID={3}";
        public const string SearchProduct = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&s={3}";

        public const int Success = 0;
        public const int Failed = -1;
        public const int Exists = 1;

        public const string Active = "1";
        public const string InActive = "0";

        public const int Yes = 1;
        public const int No = 0;

        //Profile
        public const string LINK_UPLOAD_AVATAR = "~/Content/Img/Avartar/";
        public static string LINK_UPLOAD_AVATAR_DEFAULT = "~/Content/Img/Avartar/no-avatar.png";

        public const int MaxItem = 12;
    }
}