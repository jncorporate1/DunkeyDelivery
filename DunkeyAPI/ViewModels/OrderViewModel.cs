using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        public int UserId { get; set; }

       
        public DateTime? DeliveryDateTime_From { get; set; }

       
        public DateTime? DeliveryDateTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public UInt16 PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public double TipAmount { get; set; }

        public OrderViewModel()
        {
            Cart = new CartViewModel();
        }
        public CartViewModel Cart { get; set; }
    }
}