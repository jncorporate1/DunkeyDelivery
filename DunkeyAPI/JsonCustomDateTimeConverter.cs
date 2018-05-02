using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI
{
    public class JsonCustomDateTimeConverter : IsoDateTimeConverter
    {
        public JsonCustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd hh:mm:ss";
        }
    }
}