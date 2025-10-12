namespace InventoryDashboard.Infrastructure.Exceptions
{
    using System;

    public abstract class BaseException : Exception
    {
        protected BaseException()
        {
        }

        protected BaseException(Exception inner, string message)
            : base(message, inner)
        {
        }

        protected BaseException(string message)
            : base(message)
        {
        }

        protected BaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
