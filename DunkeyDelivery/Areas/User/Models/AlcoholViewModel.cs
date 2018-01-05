using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class AlcoholViewModel : BaseViewModel
    {
        public AlcoholViewModel()
        {
            storeViewModel = new ShopViewModel();
            categoryViewModel = new CategorySubCategoryVM();
            UserReview = new ReviewBindingModel();
            ReviewForView = new ReviewViewModel();
        }
        public ReviewBindingModel UserReview { get; set; }
        public ReviewViewModel ReviewForView { get; set; }
        public ShopViewModel storeViewModel { get; set; }
        //public CategoriesViewModel categoryViewModel { get; set; }
        // public CategorySubCategoryVM categoryViewModel { get; set; }
         public CategorySubCategoryVM categoryViewModel { get; set; }
    }

    public class CategorySubCategoryVM
    {
        public string Name { get; set; }
        public List<CategoryViewModel> CategoryViewModel { get; set; }
    }



    // products of alcohol stores 

    public class AlcoholProductVM
    {
        public AlcoholProductVM()
        {
            productslist = new List<productslist>();
        }
        public string Name { get; set; }
        public StoreViewModel shopViewModel { get; set; }
        public List<productslist> productslist { get; set; }
    }
    public class productslist
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public string Size { get; set; }

        public string Image { get; set; }

        public int? Category_id { get; set; }

        public int Store_id { get; set; }

        public void Dispose()
        {

        }
    }



}