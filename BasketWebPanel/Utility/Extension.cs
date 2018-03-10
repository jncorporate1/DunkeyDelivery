using BasketWebPanel.Areas.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.Extensions
{
    public static class ExtensionMethods
    {
       

        public static void StoreTypesToString(this AddStoreViewModel model)
        {
            try
            {
                if (model.Store.StoreDeliveryTypes.Count > 0)
                {
                    var intCat = model.Store.StoreDeliveryTypes.Select(i => i.Type_Id).ToArray();
                    model.Store.DeliveryTypeStringIds = string.Join(",", intCat);
                    model.Store.DeliveryTypeStringIds = "[" + model.Store.DeliveryTypeStringIds + "]";
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
    }
}