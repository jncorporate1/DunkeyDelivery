using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ClientViewModel
{
    public class StoreTagViewModel 
    {
        public StoreTagViewModel(StoreTags model)
        {
            Id = model.Id;
            Tag = model.Tag;
            Store_Id = model.Store_Id;
        }

        public int Id { get; set; }
        public string Tag { get; set; }

        public int Store_Id { get; set; }
        [JsonIgnore]
        public Store Store { get; set; }


    }
}