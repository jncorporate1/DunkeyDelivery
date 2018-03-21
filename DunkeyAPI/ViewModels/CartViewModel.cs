using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            CartItems = new List<CartItemViewModel>();
        }
        public List<CartItemViewModel> CartItems { get; set; }
    }

    public class CategoryAndSubCategoryIds
    {
        public ICollection<int> Ids { get; set; }

    }
}