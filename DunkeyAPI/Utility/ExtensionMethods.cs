using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;

namespace DunkeyAPI.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void SetAverageRating(this Store store)
        {
            try
            {
                if (store.StoreRatings.Count > 0)
                {
                    store.AverageRating = store.StoreRatings.Average(x => x.Rating);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}