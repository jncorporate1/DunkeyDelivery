using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Settings
    {
        [JsonIgnore]
        public int Id { get; set; }
        public double DeliveryFee { get; set; }
        public string Currency { get; set; }
        public double Point { get; set; }
        public double Tip { get; set; }
        public string ContactNo { get; set; }
    }
}
