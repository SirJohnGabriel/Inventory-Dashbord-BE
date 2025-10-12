namespace InventoryDashboard.Infrastructure.Exceptions
{
    using System;

    public class IdentityServiceException : BaseException
    {
        public IdentityServiceException(Exception ex, string message = "")
            : base(ex, message ?? ex.Message)
        {
        }

        public IdentityServiceException()
        {
        }

        public IdentityServiceException(string message)
            : base(message)
        {
        }

        public IdentityServiceException(string message, Exception innnerException)
            : base(message, innnerException)
        {
        }
    }
}
