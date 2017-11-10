using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class OrderSummaryViewModel
    {
        public OrderSummaryViewModel(Order order)
        {
            OrderId = order.Id;
            OrderDateTime = order.OrderDateTime;
            DeliveryDateTime_From = order.DeliveryTime_From;
            DeliveryDateTime_To = order.DeliveryTime_To;
            PaymentMethodType = order.PaymentMethod;
            ServiceFee = order.ServiceFee;
            DeliveryFee = order.DeliveryFee;
            
        }

        public int OrderId { get; set; }

        public DateTime? OrderDateTime { get; set; }

        public DateTime? DeliveryDateTime_From { get; set; }

        public DateTime? DeliveryDateTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public double SubTotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double TotalTaxDeducted { get; set; }

        public string DeliveryDetails_FirstName { get; set; }
        public string DeliveryDetails_LastName { get; set; }
        public string DeliveryDetails_Phone { get; set; }
        public string DeliveryDetails_ZipCode { get; set; }
        public string DeliveryDetails_Email { get; set; }
        public string DeliveryDetails_City { get; set; }
        public string DeliveryDetails_Address { get; set; }
        public string DeliveryDetails_AddtionalNote { get; set; }

    }
}