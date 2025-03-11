using System.Net;

namespace WebAPI_Code_First.Utilities
{
    public class APIMessage
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public object Data { get; set; }

        public APIMessage(HttpStatusCode statusCode, string message, object data = null)
        {
            StatusCode = (int)statusCode;
            StatusMessage = message;
            Data = data;
        }
    }
}
