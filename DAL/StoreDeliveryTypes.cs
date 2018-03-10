using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class StoreDeliveryTypes
    {

        public int Id { get; set; }

        public int Type_Id { get; set; }

        public string Type_Name { get; set; }
         
        public int? Store_Id { get; set; }

        [JsonIgnore]
        public virtual Store Store { get; set; }
    }
}
