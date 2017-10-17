using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class OfferProductAndPackagesViewModel
    {
        public OfferProductAndPackagesViewModel()
        {
            Products = new List<OfferProductViewModel>();
            Packages = new List<OfferPackageViewModel>();
        }

        public List<OfferProductViewModel> Products { get; set; }
        public List<OfferPackageViewModel> Packages { get; set; }
    }

    public class OfferPackageViewModel
    {
        public int Id { get; set; }

        public bool NoUse { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public short Status { get; set; }

        public int Store_Id { get; set; }

        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }

        public string StoreName { get; set; }

        public bool IsChecked { get; set; }

        public double Price { get; set; }

        public int Qty { get; set; } = 1;

        public int OfferPackageId { get; set; }

        public int Package_Id { get; set; }

    }

    public class OfferProductViewModel
    {
        public int Id { get; set; }

        public bool NoUse { get; set; }

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

        public bool IsChecked { get; set; }

        public int OfferProductId { get; set; }

        public int Package_Id { get; set; }

        public int Product_Id { get; set; }

    }
}