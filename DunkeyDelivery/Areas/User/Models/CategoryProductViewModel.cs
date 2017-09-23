using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class CategoryProductViewModel : BaseViewModel
    {
        public ShopViewModel shopViewModel { get; set; }
        public IEnumerable<CategoryViewModel> Categories { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<DeliveryHours> DeliveryHours { get; set; }

    }



    public class LaundryProductsInCategory : BaseViewModel
    {
        public LaundryProductsInCategory()
        {
            laundryproducts = new List<LundryProductVM>();
            StoreViewModel = new ShopViewModel();
        }
        public List<LundryProductVM> laundryproducts = new List<LundryProductVM>();
        public ShopViewModel StoreViewModel { get; set; }

    }
    public class LundryProductVM
    {
        public LundryProductVM()
        {
            Products = new List<ProductViewModel>();

        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public int? ParentCategoryId { get; set; }

        public List<ProductViewModel> Products { get; set; }

    }
}