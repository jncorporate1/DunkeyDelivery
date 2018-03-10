using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;
using BasketWebPanel.ViewModels;
using System.Web.Mvc;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddStoreViewModel : BaseViewModel
    {
        public AddStoreViewModel()
        {
            Store = new StoreViewModel();
        }

        public StoreViewModel Store { get; set; }
        public SelectList StoreDeliveryTypes { get; internal set; }
    }

    public class StoreViewModel
    {
        public StoreViewModel()
        {
            StoreDeliveryHours = new StoreDeliveryHoursViewModel();
            StoreDeliveryTypes = new List<StoreDeliveryTypes>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Store Name is required")]
        public string BusinessName { get; set; }

        [Required(ErrorMessage = "Store Type is required")]
        public string BusinessType { get; set; }

        [Required(ErrorMessage = "Please select location from map")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Please select location from map")]
        public double Longitude { get; set; }


        public string ImageUrl { get; set; }

        public string BannerUrl { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }

        //[Required(ErrorMessage = "This field is required")]
        //[RegularExpression(MyRegularExpressions.Minutes, ErrorMessage = "Please enter a valid delivery time")]
        public int MinDeliveryTime { get; set; }


        [Required(ErrorMessage = "This field is required")]
        [Range(1, 1000, ErrorMessage = "Please enter a valid delivery charges")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a delivery charges")]
        public decimal? MinDeliveryCharges { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Range(1, 1000, ErrorMessage = "Please enter a valid order price")]
        [RegularExpression(MyRegularExpressions.Price, ErrorMessage = "Please enter a order price")]
        public float? MinOrderPrice { get; set; }

        public double AverageRating { get; set; }
   
        public List<int> DeliveryType_Id { get; set; }

        public string DeliveryTypeStringIds { get; set; }

        public List<StoreDeliveryTypes> StoreDeliveryTypes { get; set; }
        
        public StoreDeliveryHoursViewModel StoreDeliveryHours { get; set; }

    }
    public class StoreDeliveryTypes
    {
        public int Id { get; set; }

        public int Type_Id { get; set; }

        public string Type_Name { get; set; }

        public int Store_Id { get; set; }
    }
    public class StoreDeliveryHoursViewModel
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{hh:mm tt}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public TimeSpan Monday_From { get; set; }

        public TimeSpan Monday_To { get; set; }
        public TimeSpan Tuesday_From { get; set; }
        public TimeSpan Tuesday_To { get; set; }
        public TimeSpan Wednesday_From { get; set; }
        public TimeSpan Wednesday_To { get; set; }
        public TimeSpan Thursday_From { get; set; }
        public TimeSpan Thursday_To { get; set; }
        public TimeSpan Friday_From { get; set; }
        public TimeSpan Friday_To { get; set; }
        public TimeSpan Saturday_From { get; set; }
        public TimeSpan Saturday_To { get; set; }
        public TimeSpan Sunday_From { get; set; }
        public TimeSpan Sunday_To { get; set; }
    }
}