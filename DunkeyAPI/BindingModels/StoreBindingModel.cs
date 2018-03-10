using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.BindingModels
{
    public class StoreBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [Required]
        public string BusinessType { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public float? MinOrderPrice { get; set; }


        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        public string ImageUrl { get; set; }

        public string DeliveryTypes { get; set; }

        public List<int> DeliveryTypes_Id { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }
        //public DbGeography Location { get; set; }

        public StoreDeliveryHours StoreDeliveryHours { get; set; }

        public bool ImageDeletedOnEdit { get; set; }
    }
}