using BasketWebPanel.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.ViewModels
{
    public class AddOfferViewModel : BaseViewModel
    {
        public AddOfferViewModel()
        {
            Offer = new OfferViewModel();
            Products = new List<OfferProductViewModel>();
            Packages = new List<OfferPackageViewModel>();
        }

        public OfferViewModel Offer { get; set; }
        public List<OfferProductViewModel> Products { get; set; }
        public List<OfferPackageViewModel> Packages { get; set; }

        public SelectList StoreOptions { get; set; }
    }
}