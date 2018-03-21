using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class ShopCategoriesViewModel 
    {
        public List<Category> Categories { get; set; }
    }
    public class StoreDeliveryTypeList
    {
        public StoreDeliveryTypeList()
        {
            StoreScheduleList=new List<StoreDeliveryTypes>();
            Store = new Store();
        }
        public Store Store { get; set; }
        public List<StoreDeliveryTypes> StoreScheduleList { get; set; }
    }
}