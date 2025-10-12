namespace InventoryDashboard.Infrastructure.Messages
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public sealed class Response<T>
    {
        public Response(T data)
        {
            this.Data = data;
        }

        public Response()
        {
            this.ErrorCode = string.Empty;
            this.Message = string.Empty;
        }

        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public string ErrorCode { get; private set; }

        [DataMember]
        public T Data { get; set; }

        public void SetError(string errorCode, string error = "")
        {
            this.ErrorCode = errorCode;
            this.Message = error;
        }

        public void SetError(string errorCode, ICollection<string> errors)
        {
            this.ErrorCode = errorCode;
            this.Message = string.Join(". ", errors);
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
    public sealed class Response
    {
        public Response()
        {
            this.ErrorCode = string.Empty;
            this.Message = string.Empty;
        }

        [DataMember]
        public string Message { get; private set; }

        [DataMember]
        public string ErrorCode { get; private set; }

        public void SetError(string errorCode, string error = "")
        {
            this.ErrorCode = errorCode;
            this.Message = error;
        }

        public void SetError(string errorCode, ICollection<string> errors)
        {
            this.ErrorCode = errorCode;
            this.Message = string.Join(". ", errors);
        }
    }
}
