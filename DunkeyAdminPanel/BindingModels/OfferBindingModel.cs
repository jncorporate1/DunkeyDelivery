using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class OfferBindingModel
    {
        public int Id { get; set; }
        
        public DateTime ValidFrom { get; set; }
        
        public DateTime ValidUpto { get; set; }
        
        public string Title { get; set; }

        public string Description { get; set; }
        
        public int Status { get; set; }

        public string ImageUrl { get; set; }

        public int Store_Id { get; set; }
    }
}