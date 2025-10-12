namespace InventoryDashboard.Infrastructure.Extensions
{
    using System;

    public static class ExceptionExtension
    {
        public static Exception GetInnerMostException(this Exception ex)
        {
            var result = ex;

            if (result != null)
            {
                while (result.InnerException != null)
                {
                    result = result.InnerException;
                }
            }

            return result;
        }
    }
}