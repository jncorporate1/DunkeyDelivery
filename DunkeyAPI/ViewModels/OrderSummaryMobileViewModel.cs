using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class OrderSummaryMobileViewModel
    {
        public double SubTotal { get; set; } = 0;

        public double SubTotalWDF { get; set; } = 0; // without delivery fee

        public double Tax { get; set; } = 0;

        public double Tip { get; set; } = 0;

        public double Total { get; set; } = 0;

        public double DeliveryFee { get; set; } = 0;

        

    }
}