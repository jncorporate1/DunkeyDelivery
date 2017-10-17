using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.BindingModels
{
    public class SearchOfferBindingModel
    {
        public string OfferName { get; set; } = "";
        public int? StoreId { get; set; }
        public int? OfferId { get; set; }
        public SelectList StoreOptions { get; internal set; }
        public string ProductName { get; internal set; }
        public string PackageName { get; internal set; }
        public string ProductPrice { get; internal set; }
        public string CategoryName { get; internal set; }
    }
}