using DunkeyDelivery.ViewModels;
using Newtonsoft.Json;
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

        public double Total { get; set; }

        public double TipAmount { get; set; }

        public double TotalTaxDeducted { get; set; }

        public OrderViewModel()
        {
            Cart = new CartViewModel();
            DeliveryDetails = new DeliveryDetails();
            PaymentInformation = new PaymentInormation();
        }

        public CartViewModel Cart { get; set; }

        public DeliveryDetails DeliveryDetails { get; set; }

        public PaymentInormation PaymentInformation { get; set; }

        public string StripeAccessToken { get; set; }
        public string StripeEmail { get; set; }

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

    // Order History View Models

    public class OrdersHistoryViewModel : BaseViewModel
    {
        public int Count { get; set; }
        public List<OrderVM> orders { get; set; }
        public int TotalOrders { get; set; }
    }

    public class OrderVM
    {
        public OrderVM()
        {
            StoreOrders = new HashSet<StoreOrderViewModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public DateTime OrderDateTime { get; set; }

        public DateTime DeliveryTime_From { get; set; }

        public DateTime DeliveryTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public string DeliveryAddress { get; set; }

        public double TotalTaxDeducted { get; set; }

        public int? DeliveryMan_Id { get; set; }

        public string UserFullName { get; set; }

        public virtual ICollection<StoreOrderViewModel> StoreOrders { get; set; }
    }


    public class StoreOrderViewModel
    {
        public StoreOrderViewModel()
        {
            OrderItems = new List<OrderItemViewModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public int Store_Id { get; set; }

        public double Subtotal { get; set; }

        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }

        public string StoreName { get; set; }

        public string ImageUrl { get; set; }

        public List<OrderItemViewModel> OrderItems { get; set; }
    }

    public class OrderItemViewModel
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int Qty { get; set; }

        public int StoreOrder_Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        //public string Weight { get; set; }

        //public double WeightInGrams { get; set; }

        //public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }
        public bool IsFavourite { get; internal set; }
    }


    public class ClothRequestBindingModel
    {
        public int store_Id { get; set; }

        public int User_Id { get; set; }

    }
}