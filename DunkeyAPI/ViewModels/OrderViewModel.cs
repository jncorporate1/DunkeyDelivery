﻿using Newtonsoft.Json;
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
            StoreDeliverytype = new List<StoreDeliverytypes>();
        }
        public List<StoreDeliverytypes> StoreDeliverytype { get; set; }
        public CartViewModel Cart { get; set; }
        public DeliveryDetailsBindingModel DeliveryDetails { get; set; }

        public int? Frequency { get; set; } = 0;

        public string StripeAccessToken { get; set; }
        public string StripeEmail { get; set; }



    }

    public class StoreDeliverytypes
    {
        public int Store_Id { get; set; }
        public int Type_Id { get; set; }
        //[JsonConverter(typeof(DAL.JsonCustomDateTimeConverter))]
        public DateTime? OrderDateTime { get; set; }
        public string OrderDate { get; set; }
        public int? MinDeliveryTime { get; set; }
    }

    public class DeliveryDetailsBindingModel
    {
        //[Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        //[Required(ErrorMessage = "Phone Number is required")]
        public string Phone { get; set; }


        //[Required(ErrorMessage = "Email Address is required")]
        //[EmailAddress(ErrorMessage = "Email Address is invalid")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        //[Required(ErrorMessage = "Zip Code is required")]
        public string ZipCode { get; set; }


        public string AdditionalNote { get; set; }

    }

    public class RequestClothBindingModel
    {
        public int Store_Id { get; set;}

        public int User_Id { get; set; }
    }

    public class RequestClothMobileBindingModel
    {
        public int Store_Id { get; set; }

        public int User_Id { get; set; }

        public double Weight { get; set; }

        public string AdditionalNote { get; set; } = "";


        public DateTime? PickUpTime { get; set; }


    }
}