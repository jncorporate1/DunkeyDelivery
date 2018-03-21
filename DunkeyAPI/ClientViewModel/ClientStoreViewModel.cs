using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ClientViewModel
{
    public class ClientStoreViewModel
    {
        public ClientStoreViewModel()
        {
            PopularStores = new List<PopularStores>();
            NearByStores = new List<NearByStores>();
        }
        public List<PopularStores> PopularStores { get; set; }
        public List<NearByStores> NearByStores { get; set; }

    }
    public class PopularStores
    {
        public PopularStores(Store model)
        {
            Id = model.Id;
            BusinessType = model.BusinessType;
            Description = model.Description;
            BusinessName = model.BusinessName;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
            AverageRating = model.AverageRating;
            ImageUrl = model.ImageUrl;
            Address = model.Address;
            MinOrderPrice = model.MinOrderPrice;
            storeTags = model.StoreTags;
            ContactNumber = model.ContactNumber;
            storeDeliveryHours = model.StoreDeliveryHours;
            MinDeliveryTime = model.MinDeliveryTime;
            Open_To = model.Open_To;
            Open_From = model.Open_From;
            MinDeliveryCharges = model.MinDeliveryCharges;

        }

        public PopularStores(Store model,double distance)
        {
            Id = model.Id;
            BusinessType = model.BusinessType;
            Description = model.Description;
            BusinessName = model.BusinessName;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
            AverageRating = model.AverageRating;
            ImageUrl = model.ImageUrl;
            Address = model.Address;
            MinOrderPrice = model.MinOrderPrice;
            storeTags = model.StoreTags;
            Distance = distance;
            ContactNumber = model.ContactNumber;
            storeDeliveryHours = model.StoreDeliveryHours;
            MinDeliveryTime = model.MinDeliveryTime;
            Open_To = model.Open_To;
            Open_From = model.Open_From;
            MinDeliveryCharges = model.MinDeliveryCharges;

        }
        public int Id { get; set; }


        [Required]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [Required]
        public string BusinessName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }
        
        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public double Distance { get; set; } = 0;

        public string ContactNumber { get; set; }

        public string ImageUrl { get; set; }
        
        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }

        public string Address { get; set; }

        public float? MinOrderPrice { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public ICollection<StoreTags> storeTags { get; set; }

        public StoreDeliveryHours storeDeliveryHours { get; set; }
    }
    public class NearByStores
    {
        public NearByStores(Store model)
        {
            Id = model.Id;
            BusinessType = model.BusinessType;
            Description = model.Description;
            BusinessName = model.BusinessName;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
            AverageRating = model.AverageRating;
            ImageUrl = model.ImageUrl;
            Address = model.Address;
            MinOrderPrice = model.MinOrderPrice;
            storeTags = model.StoreTags;
            storeDeliveryHours = model.StoreDeliveryHours;
            ContactNumber = model.ContactNumber;
            MinDeliveryTime = model.MinDeliveryTime;
            Open_To = model.Open_To;
            Open_From = model.Open_From;
            MinDeliveryCharges = model.MinDeliveryCharges;

        }

        public NearByStores(Store model,double distance)
        {
            Id = model.Id;
            BusinessType = model.BusinessType;
            Description = model.Description;
            BusinessName = model.BusinessName;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
            AverageRating = model.AverageRating;
            ImageUrl = model.ImageUrl;
            Address = model.Address;
            MinOrderPrice = model.MinOrderPrice;
            storeTags = model.StoreTags;
            storeDeliveryHours = model.StoreDeliveryHours;
            Distance = distance;
            ContactNumber = model.ContactNumber;
            MinDeliveryTime = model.MinDeliveryTime;
            MinOrderPrice = model.MinOrderPrice;
            MinDeliveryCharges = model.MinDeliveryCharges;
            Open_To = model.Open_To;
            Open_From = model.Open_From;

        }
        public int Id { get; set; }


        [Required]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [Required]
        public string BusinessName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }

        [NotMapped]
        public double Distance { get; set; } = 0;

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }

        public float? MinOrderPrice { get; set; }

        public int MinDeliveryTime { get; set; }

        public ICollection<StoreTags> storeTags { get; set; }

        public StoreDeliveryHours storeDeliveryHours { get; set; }


    }

}