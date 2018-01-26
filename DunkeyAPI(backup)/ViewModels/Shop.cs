using System;
using DAL;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;

namespace DunkeyAPI.ViewModels
{
    public class Shop
    {
        public Shop()
        {
            Store = new List<Store>();
        }
        public string ErrorMessage { get; set; } = "";
        public IEnumerable<Store> Store { get; set; }
        public int TotalStores { get; set; }
    }
}