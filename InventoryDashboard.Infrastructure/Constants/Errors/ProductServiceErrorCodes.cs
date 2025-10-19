namespace InventoryDashboard.Infrastructure.Constants.Errors
{
    public static class ProductServiceErrorCodes
    {
        public const string ProductAlreadyExists = "PRODUCT_ALREADY_EXISTS";
        public const string ProductNotFound = "PRODUCT_NOT_FOUND";
        public const string UnexpectedError = "UNEXPECTED_ERROR";
        public const string ValidationError = "VALIDATION_ERROR";
        public const string LookupDataNotFound = "LOOKUP_DATA_NOT_FOUND";
        public const string AuthenticationFailed = "AUTHENTICATION_FAILED";
        public const string CurrencyCodeNotSupported = "CURRENCY_CODE_NOT_SUPPORTED";
    }
}