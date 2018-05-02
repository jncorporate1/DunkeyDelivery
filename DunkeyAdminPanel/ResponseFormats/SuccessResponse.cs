using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel
{
    public class CustomResponse<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Result { get; set; }
    }

    [JsonObject(Title = "Error")]
    class Error : JObject
    {
        //public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}