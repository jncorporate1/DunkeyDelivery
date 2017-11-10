using DunkeyDelivery.ViewModels;
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
        public int TotalCartItems { get; set; }
        public float? MinOrder { get; set; } = 0;
    }
    public class StoreItem
    {
        public StoreItem()
        {
            CartItems = new List<CartItem>();
        }
        public int StoreId { get; set; }
        public string StoreName { get; set; }
        public float MinDelivery { get; set; }
        public List<CartItem> CartItems { get; set; }
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