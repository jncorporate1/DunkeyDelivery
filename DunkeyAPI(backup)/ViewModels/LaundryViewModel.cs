using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class LaundryViewModel
    {
        
    }
    public class LaundryCategoriesViewModel
    {
        public LaundryCategoriesViewModel()
        {
            Categories = new List<Category>();
        }
        public List<Category> Categories { get; set; }
    }

    public class LaundryProductsViewModel
    {
        public LaundryProductsViewModel()
        {
            Products = new List<Product>();
        }

        public List<Product> Products { get; set; }
    }
}