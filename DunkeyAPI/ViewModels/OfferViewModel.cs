using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class OfferViewModel
    {
        public IEnumerable<Offer_Packages> Offer_Packages { get; set; }
        public IEnumerable<Offer_Products> Offer_Products { get; set; }
    }
}