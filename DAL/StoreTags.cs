using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class StoreTags
    {
        public int Id { get; set; }
        public string Tag { get; set; }

        public int Store_Id { get; set; }

        [JsonIgnore]
        public Store Store { get; set; }
    }
}
