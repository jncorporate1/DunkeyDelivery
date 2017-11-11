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
            DeliveryDetails = new DeliveryDetailsBindingModel();
        }

        public CartViewModel Cart { get; set; }
        public DeliveryDetailsBindingModel DeliveryDetails { get; set; }
    }

    public class DeliveryDetailsBindingModel
    {
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string Phone { get; set; }


        [Required(ErrorMessage = "Email Address is required")]
        [EmailAddress(ErrorMessage = "Email Address is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        public string ZipCode { get; set; }


        public string AdditionalNote { get; set; }

    }
}