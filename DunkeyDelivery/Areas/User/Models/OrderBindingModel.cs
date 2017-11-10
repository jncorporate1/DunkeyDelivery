using DunkeyDelivery.Areas.Dashboard.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class OrderBindingModel : DunkeyDelivery.ViewModels.BaseViewModel
    {
        public OrderBindingModel()
        {
            StoreOrders = new HashSet<StoreOrderBindingModel>();
        }
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_From { get; set; }

        //[JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_To { get; set; }

        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double TotalTaxDeducted { get; set; }

        public double TipAmount { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public short PaymentStatus { get; set; }
        
        public virtual ICollection<StoreOrderBindingModel> StoreOrders { get; set; }
        
        public int? DeliveryMan_Id { get; set; }
        public bool RemoveFromDelivererHistory { get; set; }
        public string DeliveryDetails_FirstName { get; set; }
        public string DeliveryDetails_LastName { get; set; }
        public string DeliveryDetails_Phone { get; set; }
        public string DeliveryDetails_ZipCode { get; set; }
        public string DeliveryDetails_Email { get; set; }
        public string DeliveryDetails_City { get; set; }
        public string DeliveryDetails_Address { get; set; }
        public string DeliveryDetails_AddtionalNote { get; set; }
    }

    public class StoreOrderBindingModel
    {
        public StoreOrderBindingModel()
        {
            Order_Items = new HashSet<OrderItemBindingModel>();
        }

        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public int Store_Id { get; set; }

        public double Subtotal { get; set; }
        
        public double Total { get; set; }

        public bool IsDeleted { get; set; }

        public int Order_Id { get; set; }
        
        public virtual ICollection<OrderItemBindingModel> Order_Items { get; set; }
    }

    public class OrderItemBindingModel
    {
        public int Id { get; set; }
        
        public int Qty { get; set; }

        public int? Product_Id { get; set; }

        public int? Package_Id { get; set; }

        public int? Offer_Product_Id { get; set; }

        public int? Offer_Package_Id { get; set; }

        public int StoreOrder_Id { get; set; }
        
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }
    }
}