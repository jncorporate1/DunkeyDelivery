using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddProductViewModel : BaseViewModel
    {
        public AddProductViewModel()
        {
            StoreOptions = new SelectList(new List<SelectListItem>());
            CategoryOptions = new SelectList(new List<SelectListItem>());
            WeightOptions = new SelectList(new List<SelectListItem>());
            Product = new ProductBindingModel();
        }
        
        public SelectList StoreOptions { get; set; }

        public SelectList CategoryOptions { get; internal set; }
        
        public ProductBindingModel Product { get; set; }

        public SelectList WeightOptions { get; internal set; }

    }
}