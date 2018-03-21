using DAL;
using System.Collections.Generic;

namespace DunkeyAPI.Models
{
    //public class FilterStoreViewModel
    //{
    //   public FilterStoreViewModel()
    //    {
    //        Stores = new List<FilterStores>();
    //    }
    //    public List<FilterStores> Stores { get; set; }
    //}

    public class FilterStoreViewModel
    {
        public FilterStoreViewModel()
        {
            NearByStores = new List<FilterStores>();
            PopularStores = new List<FilterStores>();

        }
        public List<FilterStores> NearByStores { get; set; }
        public List<FilterStores> PopularStores { get; set; }

    }
    public class FilterStores
    {
        public FilterStores()
        {
            StoreRatings = new List<StoreRatings>();
        }
        public int Id { get; set; }

        public string BusinessType { get; set; }

        public string Description { get; set; }

        public string BusinessName { get; set; }

        public double Distance { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int Category_Id { get; set; }

        public double? AverageRating { get; set; }

        //public int AverageRating { get; set; }

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public float? MinOrderPrice { get; set; }

        public bool IsDeleted { get; set; }

        public List<StoreRatings> StoreRatings { get; set; }

        public ICollection<StoreTags> storeTags { get; set; }
    }


    public class Filter
    {
        public int Store_Id { get; set; }

        public string BusinessName { get; set; }

    }

}