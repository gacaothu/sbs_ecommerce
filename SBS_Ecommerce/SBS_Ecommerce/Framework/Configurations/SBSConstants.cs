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
        public const string GetListSearchProduct = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&s={3}";

        public const string GetListPromotion = Domain + "/WServ/LstProduct?cID={0}&type={1}";
        public const string GetListArrivals = Domain + "/WServ/LstProduct?cID={0}&type={1}";
        public const string GetBestSellerProduct = Domain + "/WServ/LstProduct?cID={0}&type={1}";

        public const string GetListCategory = Domain + "/wserv/LstCategory?plength={0}&pno={1}&sort={2}&cID={3}";
        public const string GetListChildCategory = Domain + "/wserv/LstChildCategory?cgID={0}&plength={1}&pno={2}&sort={3}&cID={4}";

        public const string GetProduct = Domain + "/WServ/GetProduct?pID={0}";
        public const string GetListProductByCategory = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&cgID={3}";
        public const string SearchProductWithoutCategory = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&s={3}&sort={4}&sorttype={5}";
        public const string SearchProductWithCategory = Domain + "/WServ/LstProduct?cID={0}&pno={1}&plength={2}&s={3}&sort={4}&sorttype={5}&cgID={6}";
        public const string GetTags = Domain + "/WServ/LstTag?cID={0}";
        public const string GetCompany = Domain + "/WServ/GetCompany?d={0}";
        public const string GetBrand = Domain + "/WServ/LstBrand?cID={0}";
        public const string GetPriceRange = Domain + "/WServ/LstProductPriceRange?cID={0}";
        public const string GetListBank = Domain + "/WServ/LstBank?ctryID={0}";
        public const string GetListBankAcount = Domain + "/WServ/LstBankAccount?cID={0}";
        public const string GetListProductReview = Domain + "/Wserv/LstProductReview?pID={0}";
        public const string GetProfile = Domain + "/Wserv/GetProfile?pID={0}";

        public const int Success = 0;
        public const int Failed = -1;
        public const int Exists = 1;
        public const int LoginRequired = 99;

        public const string Active = "1";
        public const string InActive = "0";

        public const int Yes = 1;
        public const int No = 0;

        //Profile
        public const string LINK_UPLOAD_AVATAR = "~/Content/img/avartar/";
        public const string LINK_UPLOAD_PAYSLIP = "~/Content/img/payslip/";
        public const string LINK_UPLOAD_AVATAR_DEFAULT = "~/Content/img/avartar/no-avatar.png";
        public const string LINK_APILOGIN = Domain + "/WServ/Login";
        public const string LINK_APIFORGOTPASSOWRD = Domain + "/WServ/forgotpassword?e=";
        public const string LINK_API_GET_TAX = Domain + "/WServ/GetTax?cID={0}";
        public const string LINK_API_GET_PROMOTION = Domain + "/WServ/GetPromotionCoupon?cID={0}&code={1}&{2}";
        public const string LINK_API_CONVERT_MONNEY = "http://api.fixer.io/latest?symbols=USD,{0}";
        public const string SESSION_COMPANYID = "session_companyid";
        public const string LINK_API_STOCKOUT = Domain + "/Wserv/StockOut";

        public const int MaxItem = 12;

        public const string SessionCategoryKey = "TempProductByCategory";
        public const string SessionSearchProductKey = "TempSearchProducts";
        public const string SessionSearchKey = "SearchKey";
        public const string SessionCategory = "Category";
        public const string SessionProduct = "Product";
        public const string SessionPriceRange = "PriceRange";
        public const string SessionBrand = "Brand";
        public const string SessionConfigChatting = "Chatting";
        public const string SessionUser = "CachedUser";

        public const string PathUploadSlider = "~/Content/img/slider/";
        public const string PathUploadBlog = "~/Content/img/blog/";

        public const string PathPage = "~/Page/Index/";
    }
}