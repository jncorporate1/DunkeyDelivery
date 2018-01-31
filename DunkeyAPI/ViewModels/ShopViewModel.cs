using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class ShopViewModel : IDisposable
    {
        public ShopViewModel(Store model)
        {

            BusinessName = model.BusinessName;
            BusinessType = model.BusinessType;
            Description = model.Description;
            Longitude = model.Longitude;
            Latitude = model.Latitude;

        }
        

        public string Description { get; set; }

        public string BusinessName { get; set; }

        public string BusinessType { get; set; }

        public string Image { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public StoreTags StoreTags { get; set; }
        //public virtual ICollection<StoreTags> StoreTags { get; set; }

        //  public virtual ICollection<Category> Category { get; set; }


        public void Dispose()
        {

        }
    }


    public class SideBar
    {
        public SideBar()
        {
            StoreCounts = new StoreCounts();
            cuisines = new List<Cuisines>();
        }
        public StoreCounts StoreCounts { get; set; }
        public List<Cuisines> cuisines { get; set; }


    }
    public class CousineViewModel
    {
        public CousineViewModel()
        {
           cuisines = new List<Cuisines>();
        }
        public List<Cuisines> cuisines { get; set; }


    }

    public class Cuisines
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int TotalCount { get; set; }
    }
    public class StoreCounts
    {
        public int TotalFoodStores { get; set; }
        public int TotalGroceryStores { get; set; }
        public int TotalAlcoholtores { get; set; }
        public int TotalLaundryStores { get; set; }
        public int TotalPharmacyStores { get; set; }
        public int TotalRetailStores { get; set; }
    }

    public class ReviewBindingModel{
        [Required]
        public int User_Id { get; set; }
        [Required]
        public int Store_Id { get; set; }
        [Required]
        public int Rating { get; set; }
        public string Feedback { get; set; }

        
    }

  
    public class ReviewList
    {
        public List<StoreRatings> Reviews { get; set; }
    }

}