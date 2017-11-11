using DunkeyDelivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    
    public class StripeSettings
    {
        public StripeSettings()
        {
            this.ReloadStripeSettings();
        }

        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}