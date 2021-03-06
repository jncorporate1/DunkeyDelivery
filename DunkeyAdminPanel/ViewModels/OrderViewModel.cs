﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class OrderViewModel : BaseViewModel
    {
        public int Id { get; set; }

        public string OrderNo { get; set; }

        public int Status { get; set; }

        public DateTime OrderDateTime { get; set; }

        public DateTime? DeliveryTime_From { get; set; }

        public DateTime? DeliveryTime_To { get; set; }

        public string AdditionalNote { get; set; }

        public int PaymentMethod { get; set; }

        public string PaymentMethodName { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double TotalTaxDeducted { get; set; }

        public double TipAmount { get; set; }

        public double Total { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public short PaymentStatus { get; set; }

        public string PaymentStatusName { get; set; }

        public string DeliveryAddress { get; set; }

        public virtual OrderPaymentViewModel OrderPayment { get; set; }

        public virtual ICollection<StoreOrderViewModel> StoreOrders { get; set; }

        public virtual UserViewModel User { get; set; }
    }

    public class OrderPaymentViewModel
    {
        public int Id { get; set; }

        public string Amount { get; set; }

        public string PaymentType { get; set; }

        public string CashCollected { get; set; }

        public string Status { get; set; }

        public string Order_Id { get; set; }

        public string AccountNo { get; set; }

        public int? DeliveryMan_Id { get; set; }

        public int Application_Id { get; set; }
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

        public string Weight { get; set; }

        public double WeightInGrams { get; set; }

        public double WeightInKiloGrams { get; set; }

        public string ImageUrl { get; set; }
        public bool IsFavourite { get; internal set; }
    }
}