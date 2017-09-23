using DunkeyDelivery.BindingModels;
using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DunkeyDelivery.Areas.Dashboard.Models
{
    public class AddCategoryViewModel : BaseViewModel
    {

        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; }

        //[Required]
        public string Description { get; set; }


        public short ParentCategoryId { get; set; }

        //[Required]
        public short Store_Id { get; set; }

        public short Status { get; set; }

        //public IEnumerable<CategoryBindingModel> ParentCats;

        public SelectList ParentCategoryOptions { get; set; }

        public SelectList StoreOptions { get; set; }
    }
}