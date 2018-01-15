using DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class AlcoholViewModel
    {
        public AlcoholViewModel()
        {
            Stores = new List<Store>();
        }

        public List<Store> Stores { get; set; }

    }

    public class StoreViewModel
    {
        public int Id { get; set; }
        
        public string BusinessType { get; set; }

        public string Description { get; set; }
        
        public string BusinessName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }
        
        public double AverageRating { get; set; }


        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public float? MinOrderPrice { get; set; }

        public bool IsDeleted { get; set; }

        public DbGeography Location { get; set; }

        public List<AlcoholCategories> Categories { get; set; }
    }

    public class AlcoholCategories
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public List<productslist> Products { get; set; }
    }
}