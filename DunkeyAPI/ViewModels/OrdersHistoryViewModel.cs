using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class OrdersHistoryViewModel
    {
        public int Count { get; set; }
        public List<OrderVM> orders { get; set; }

    }

    public class OrderVM
    {
        public OrderVM(Order order)
        {
            Id = order.Id;
            OrderDateTime = order.OrderDateTime;
            DeliveryTime_From = order.DeliveryTime_From;
            DeliveryTime_To = order.DeliveryTime_To;
            PaymentMethod = order.PaymentMethod;
            Subtotal = order.Subtotal;
            ServiceFee = order.ServiceFee;
            DeliveryFee = order.DeliveryFee;

            //DeliveryDetails
            DeliveryDetails_FirstName = order.DeliveryDetails_FirstName;
            DeliveryDetails_LastName = order.DeliveryDetails_LastName;
            DeliveryDetails_Phone = order.DeliveryDetails_Phone;
            DeliveryDetails_ZipCode = order.DeliveryDetails_ZipCode;
            DeliveryDetails_Email = order.DeliveryDetails_Email;
            DeliveryDetails_City = order.DeliveryDetails_City;
            DeliveryDetails_AddtionalNote = order.DeliveryDetails_AddtionalNote;
            DeliveryDetails_Address = order.DeliveryDetails_Address;

            //Totals
            Subtotal = order.Subtotal;
            Total = order.Total;
            TotalTaxDeducted = order.TotalTaxDeducted;
            TipAmount = order.TipAmount;

            StoreOrders = new HashSet<StoreOrderViewModel>();
        }

        public OrderVM()
        {
            StoreOrders = new HashSet<StoreOrderViewModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_From { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_To { get; set; }
        
        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double TipAmount { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public int? DeliveryMan_Id { get; set; }

        public string UserFullName { get; set; }

        public double TotalTaxDeducted { get; set; }

        public string DeliveryDetails_FirstName { get; set; }
        public string DeliveryDetails_LastName { get; set; }
        public string DeliveryDetails_Phone { get; set; }
        public string DeliveryDetails_ZipCode { get; set; }
        public string DeliveryDetails_Email { get; set; }
        public string DeliveryDetails_City { get; set; }
        public string DeliveryDetails_Address { get; set; }
        public string DeliveryDetails_AddtionalNote { get; set; }

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

        public double DeliveryFee { get; set; }

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

        public string ImageUrl { get; set; }
    }
}