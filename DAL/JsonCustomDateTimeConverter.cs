using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class JsonCustomDateTimeConverter : IsoDateTimeConverter
    {
        public JsonCustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-dd hh:mm tt";
        }
    }
}
