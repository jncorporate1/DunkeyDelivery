﻿using DAL;
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
            FilterProductSizes = new List<AlcoholSizeFilters>();
        }
        public List<AlcoholSizeFilters> FilterProductSizes { get; set; }
        public List<Store> Stores { get; set; }
        public int? TotalRecords { get; set; } = 0;


    }
   
    public class AlcoholSizeFilters
    {
        public string Unit { get; set; }
        public string Size { get; set; }
        public string NetWeight { get; set; }
        public short TypeID { get; set; }
        public int MainType { get; set; } = 2; // this parameter is only to fix front end issue on mobile side
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

    public class AlcoholStoreParentCategories
    {
        public AlcoholStoreParentCategories()
        {
            Categories = new AlcoholStoreCategories();
            WineLastProducts = new List<Product>();
            LiquorLastProducts = new List<Product>();
            BeerLastProducts = new List<Product>();
        }
        public AlcoholStoreCategories Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> WineLastProducts { get; set; }
        public List<Product> LiquorLastProducts { get; set; }
        public List<Product> BeerLastProducts { get; set; }
        public bool IsLast { get; set; } = false;
  

    }

    public class AlcoholStoreCategories
    {
        public AlcoholStoreCategories()
        {
            Wine = new List<Category>();
            Liquor = new List<Category>();
            Beer = new List<Category>();
        }

        public List<Category> Wine{ get; set; }
        public List<Category> Liquor { get; set; }
        public List<Category> Beer{ get; set; }

    }

    public class AlcoholChildCategories
    {
        public AlcoholChildCategories()
        {
            Categories = new List<Category>();
          }
        public List<Category> Categories { get; set; }


    }

    //public class ChangeStoreViewModel
    //{
    //    public ChangeStoreViewModel()
    //    {
    //        Categories = new List<Category>();
    //    }

    //    public List<Category> Categories { get; set; }
    //    public int TotalRecords { get; set; }
    //}

}