using BasketWebPanel.Areas.Dashboard.Models;
using BasketWebPanel.BindingModels;
using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class SearchProductsViewModel : BaseViewModel
    {
        public SearchProductsViewModel()
        {
            Products = new List<SearchProductViewModel>();
        }
        public List<SearchProductViewModel> Products { get; set; }
    }
    
}