using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddCategoryViewModel : BaseViewModel
    {
        public AddCategoryViewModel()
        {
            StoreOptions = new SelectList(new List<SelectListItem>());
            ParentCategoryOptions = new SelectList(new List<SelectListItem>());
            Category = new CategoryBindingModel();
        }
        
        public SelectList ParentCategoryOptions { get; set; }

        public SelectList StoreOptions { get; set; }

        public CategoryBindingModel Category { get; set; }
    }
}