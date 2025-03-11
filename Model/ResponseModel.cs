using Microsoft.AspNetCore.Http;

namespace WebAPI_Code_First.Model
{
    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }
        public string StatusText { get; set; }
        public T Data { get; set; }

        public ResponseModel(int status, string statusText, T data)
        {
            StatusCode = status;
            StatusText = statusText;
            Data = data;
        }
    }
}

