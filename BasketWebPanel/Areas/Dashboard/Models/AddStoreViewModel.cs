using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity.Spatial;
using BasketWebPanel.ViewModels;

namespace BasketWebPanel.Areas.Dashboard.Models
{
    public class AddStoreViewModel : BaseViewModel
    {
        public AddStoreViewModel()
        {
            Store = new StoreViewModel();
        }

        public StoreViewModel Store { get; set; }
    }

    public class StoreViewModel
    {
        public StoreViewModel()
        {
            StoreDeliveryHours = new StoreDeliveryHoursViewModel();
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

        public double AverageRating { get; set; }

        public StoreDeliveryHoursViewModel StoreDeliveryHours { get; set; }

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