namespace InventoryDashboard.Api.Common.Responses
{
    using System.Net;
    using System.Text.Json.Serialization;

    public class WebResponse
    {
        public WebResponse(string errorCode, string message = "")
        {
            this.ErrorCode = errorCode;
            this.Message = message;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ErrorCode { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode
        {
            get
            {
                if (string.IsNullOrEmpty(this.ErrorCode))
                {
                    return this.SuccessCode;
                }

                if (!this.ErrorCodes.TryGetValue(this.ErrorCode, out var errorCode))
                {
                    errorCode = HttpStatusCode.InternalServerError;
                }

                return errorCode;
            }
        }

        [JsonIgnore]
        public virtual HttpStatusCode SuccessCode => HttpStatusCode.OK;

        [JsonIgnore]
        public virtual Dictionary<string, HttpStatusCode> ErrorCodes => new()
        {
            { Infrastructure.Constants.Errors.ErrorCodes.Default, HttpStatusCode.BadRequest },
        };
    }

    public class WebResponse<T> : WebResponse
    {
        public WebResponse(T data, string errorCoed, string message = "")
            : base(errorCoed, message)
        {
            this.Data = data;
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; set; }
    }
}