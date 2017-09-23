using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public class ShopViewModel : BaseViewModel
    {

        public int Id { get; set; }



        [Required(ErrorMessage = "Business Name is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Business Name")]
       
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Business Type is required")]
        [DataType(DataType.Text)]
        [Display(Name = "Business Type")]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        [RegularExpression("^(?!0$).*", ErrorMessage = "Invalid Longitude")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Latitude is required")]
        [RegularExpression("^(?!0$).*", ErrorMessage = "Invalid Latitude")]
        public double Latitude { get; set; }

        public string ImageUrl { get; set; }



    }

    public class ShopCategoriesBindingModel
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Category Type")]
        public string Type { get; set; }

   
        public string Description { get; set; }

        [Required]
        public short Status { get; set; }

        [Required]
        public int Store_Id { get; set; }

        public SelectList StoreOptions { get; internal set; }

        public SelectList ParentCategoryOptions { get; set; }


    }


}