using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class StoreBindingModel
    {
        public int Id { get; set; }

        public string BusinessName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Schedule_Monday { get; set; }

        public string Schedule_Tuesday { get; set; }

        public string Schedule_Wednesday { get; set; }

        public string Schedule_Thursday { get; set; }

        public string Schedule_Friday { get; set; }

        public string ImageUrl { get; set; }

    }
}