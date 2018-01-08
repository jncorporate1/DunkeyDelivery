using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class MobileOrderViewModel
    {
        public List<MobileCart> Store { get; set; }
        public OrderSummaryMobileViewModel OrderSummary { get; set; }
    }
    public class MobileCart
    {
        public string businessType { get; set; }

        public int minDeliveryTime { get; set; }

        public decimal? minDeliveryCharges { get; set; }
        
        public float? minOrderPrice { get; set; }

        public int storeId { get; set; }

        public string storeName { get; set; }

        public double? StoreTax { get; set; } = 0;

        public double? StoreSubTotal { get; set; } = 0;

        public double? StoreTotal { get; set; } = 0;

        public List<productslist> products { get; set; }


    }
}

