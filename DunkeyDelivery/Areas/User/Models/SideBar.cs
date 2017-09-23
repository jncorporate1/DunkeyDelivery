using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class SideBar
    {
        public StoreCounts StoreCounts { get; set; }
        public List<Cuisines> cuisines { get; set; }


    }
    public class Cuisines
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public int TotalCount { get; set; }

    }
    public class StoreCounts
    {
        public int TotalFoodStores { get; set; }
        public int TotalGroceryStores { get; set; }
        public int TotalAlcoholtores { get; set; }
        public int TotalLaundryStores { get; set; }
        public int TotalPharmacyStores { get; set; }
        public int TotalRetailStores { get; set; }
    }
}