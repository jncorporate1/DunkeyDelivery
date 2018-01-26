using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Utility
{
    public static class DunkeySettings
    {
        public static int Id { get; set; }
        public static double DeliveryFee { get; set; }
        public static string Currency { get; set; }

        public static void LoadSettings()
        {
            try
            {
                using (DunkeyContext ctx = new DunkeyContext())
                {
                    var setting = ctx.Settings.FirstOrDefault();
                    if (setting != null)
                    {
                        Id = setting.Id;
                        DeliveryFee = setting.DeliveryFee;
                        Currency = setting.Currency;
                    }
                }
            }
            catch (Exception ex)
            {
                DunkeyDelivery.Utility.LogError(ex);
            }
        }

    }
}