using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class StoreBindingModel
    {
        public int Id { get; set; }

        public string BusinessName { get; set; }

        public string BusinessType { get; set; }
        
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Schedule_Monday { get; set; }

        public string Schedule_Tuesday { get; set; }

        public string Schedule_Wednesday { get; set; }

        public string Schedule_Thursday { get; set; }

        public string Schedule_Friday { get; set; }

        public string ImageUrl { get; set; }

    }
    public class SizeBindingModel : BaseViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Unit { get; set; }

        public string BusinessType { get; set; }
        
    }

    public class StoreDropDownBindingModel
    {
      public int Id { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }

        public string BusinessType { get; set; }

        public bool Selected { get; set; }

    }
}