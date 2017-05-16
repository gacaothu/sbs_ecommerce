namespace SBS_Ecommerce.Framework.Configurations
{
    public static class SBSMessages
    {
        public const string CharacterRequired = "The {0} must be at least {2} characters long.";
        public const string ConfirmPassword = "The password and confirmation password do not match.";
        //Shipping address
        public const string MessageAddShippingAddressSuccess = "Shipping address has been created successfully.";
        public const string MessageDeletedShippingAddressSuccess = "Shipping address has been deleted successfully.";
        public const string MessageUpdateShippingAddressSuccess = "Shipping address has been updated successfully.";
        //Billing address
        public const string MessageUpdateBillingAddressSuccess = "Billing address has been updated successfully.";
        public const string MessageAddBillingAddressSuccess = "Billing address has been created successfully.";
        public const string MessageDeleteBillingAddressSuccess = "Billing address has been deleted successfully.";

        public const string MessageUpdateInformationSuccess = "Customer information has been updated successfully.";

        public const string MessageIncorrectLogin = "Username or Password is incorrect.";

        public const string PropertyTooShort = "Username or Password is incorrect.";

        public const string InvalidEmail = "Invalid email.";
        public const string DuplicateEmail = "Email is exists.";

        public const string DuplicateName = "UserName is exists.";
        public const string InvalidUserName = "Invalid UserName.";
    }
}