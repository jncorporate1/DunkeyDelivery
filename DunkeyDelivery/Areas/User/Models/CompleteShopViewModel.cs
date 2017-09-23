using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class CompleteShopViewModel
    {
        public ShopViewModel ShopModel { get; set; }
        public IEnumerable<DeliveryHours> DeliveryHours { get; set; }
        public IEnumerable<StoreTags> StoreTags { get; set; }
        
    }
}