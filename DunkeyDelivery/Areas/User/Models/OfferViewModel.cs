using DunkeyDelivery.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class OfferViewModel : BaseViewModel
    {
        public List<Offer_Packages> Offer_Packages { get; set; }
        public List<Offer_Products> Offer_Products { get; set; }
    }

    public class Offer_Packages
    {
        public int Id { get; set; }
        public int Offer_Id { get; set; }
        public int Package_Id { get; set; }
        public string Name { get; set; }
        public DateTime ValidUpto { get; set; }
        public string Title { set; get; }
        public string ImageUrl { get; set; }
        public double Price { get; set; }
        public Package Package { get; set; }
        public Offer Offer { get; set; }
    }

    public class Offer_Products
    {
        public int Id { get; set; }
        public int Offer_Id { get; set; }
        public string Name { get; set; }
        public int Product_Id { get; set; }
        public string ImageUrl { get; set; }
        public string Title { set; get; }
        public double Price { get; set; }
        public Products Product { get; set; }
        public Offer Offer { get; set; }
    }

    public class Offer
    {
        public int Id { get; set; }
        
        public string ValidUpto { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        
        public string Status { get; set; }

        public string ImageUrl { get; set; }

        public int Store_Id { get; set; }

        public Store Store { get; set; }
    }

    public class Package
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Status { get; set; }

        public string Price { get; set; }

        public string ImageUrl { get; set; }
    }
}