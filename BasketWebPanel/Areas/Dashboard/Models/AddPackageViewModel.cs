using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    //public class AddPackageViewModel : BaseViewModel
    //{
    //    public AddPackageViewModel()
    //    {
    //        Package = new PackageBindingModel();
    //        Products = new List<PackageProductViewModel>();
    //    }

    //    public PackageBindingModel Package { get; set; }
    //    public List<PackageProductViewModel> Products;

    //}
    public class PackageProductsViewModel
    {
        public PackageProductsViewModel()
        {
            Products = new List<PackageProductViewModel>();
        }
        public List<PackageProductViewModel> Products { get; set; }
    }
  
    public class AddPackageViewModel : BaseViewModel
    {

        public AddPackageViewModel()
        {
            StoreOptions = new SelectList(new List<SelectListItem>());
            Package = new PackageBindingModel();
            Products = new List<PackageProductViewModel>();
        }

        public PackageBindingModel Package { get; set; }
        public List<PackageProductViewModel> Products { get; set; }
        public SelectList StoreOptions { get; set; }
    }

    public class PackageProductViewModel
    {
        public bool NoUse { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Weight { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public short Status { get; set; }

        public int Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public string StoreName { get; set; }

        public string CategoryName { get; set; }

        //Used in Add Package
        public int Qty { get; set; } = 1;
        
        public int QtyTemp { get; set; } = 1;

        public bool IsChecked { get; set; }

        public int PackageProductId { get; set; }

        public int Package_Id { get; set; }

        public int Product_Id { get; set; }

    }
}