using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class OrderViewModel
    {
        [Required]
        public int UserId { get; set; }


        public DateTime DeliveryDateTime_From { get; set; }


        public DateTime DeliveryDateTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public UInt16 PaymentMethodType { get; set; }

        public string DeliveryAddress { get; set; }

        public OrderViewModel()
        {
            Cart = new CartViewModel();
        }
        public CartViewModel Cart { get; set; }

    }

    public class CartViewModel
    {
        public CartViewModel()
        {
            CartItems = new List<CartItemViewModel>();
        }
        public List<CartItemViewModel> CartItems { get; set; }
    }

    public class CartItemViewModel
    {
        public int ItemId { get; set; }
        public int ItemType { get; set; }
        public int Qty { get; set; }
        public int StoreId { get; set; }
    }


}