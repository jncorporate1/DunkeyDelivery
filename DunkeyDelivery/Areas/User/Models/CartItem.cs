using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;

namespace DunkeyDelivery.Areas.User.Models
{
    public class Cart
    {
        public Cart()
        {
            Stores = new List<StoreItem>();
           // CartItems = new List<CartItem>();
        }
      //  public List<CartItem> CartItems { get; set; }
        public List<StoreItem> Stores { get; set; }
        public double Total { get; set; }
        public double Tax { get; set; }
        public int TotalCartItems { get; set; }
        public float? MinOrder { get; set; } = 0;
        public int CurrentStoreId { get; set; }
    }

    public class StoreItem
    {
        public StoreItem()
        {
            CartItems = new List<CartItem>();
            DeliveryType = new DeliveryTypeCookieBindingModel();
        }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public float MinDelivery { get; set; }
        public List<CartItem> CartItems { get; set; }
        public string BusinessType { get; set; }
        public double BusinessTypeTax { get; set; }
        public DeliveryTypeCookieBindingModel DeliveryType { get; set; }

        public class DistinctComparerOnBusinessType : IEqualityComparer<StoreItem>
        {
            public bool Equals(StoreItem x, StoreItem y)
            {
                return x.BusinessType == y.BusinessType;
            }

            public int GetHashCode(StoreItem obj)
            {
                unchecked  // overflow is fine
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.StoreId).GetHashCode();
                    return hash;
                }
            }
        }
    }
    public class DeliveryType
    {
        public int Store_Id { get; set; }
        public int Type_Id { get; set; }
        public DateTime OrderDate { get; set; }
        public TimeSpan OrderTime { get; set; }
        public bool IsTypeSet { get; set; }

    }
    public class DeliveryTypeCookieBindingModel
    {
        public int Store_Id { get; set; }
        public int Type_Id { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public int? MinDeliveryTime { get; set; }
    }
    public class CartItem
    {
        public float Price { get; set; }
        public string Name { get; set; }
        public int ItemId { get; set; }
        public string Size { get; set; }
        public int Qty = 1;
        public float Total { get; set; }
        public int Type { get; set; }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public string BusinessType { get; set; }
        public double BusinessTypeTax { get; set; }

    }

    public class ShoppingCartViewModel : BaseViewModel
    {
        public ShoppingCartViewModel()
        {
            cart = new Cart();
        }
        public Cart cart { get; set; }

    }
   
   
}