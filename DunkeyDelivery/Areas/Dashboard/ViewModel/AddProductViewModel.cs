using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public class AddProductViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Product Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Price is required.")]
        [RegularExpression(@"^\$?(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$", ErrorMessage = "Please enter a valid Price.")]
        public string Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select Category.")]
        public int Category_Id { get; set; }

        [Required]
        public int Store_Id { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public short Status { get; set; }

        public SelectList StoreOptions { get; set; }

        public SelectList CategoryOptions { get; internal set; }

        public string ImageUrl { get; set; }

    }
}