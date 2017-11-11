using DunkeyDelivery.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{

    public class RewardsViewModel
    {
        public UserPoints UserPoints;
        public List<Rewards> Rewards;
    }

    public class RewardPrize
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Rewards
    {
        public int Id { get; set; }
        public int PointsRequired { get; set; }
        public double AmountAward { get; set; }
        public string Description { get; set; }
        public int? RewardPrize_Id { get; set; }
        public RewardPrize RewardPrize { get; set; }
    }

    public class UserPoints
    {
        public int RewardPoints { get; set; }
    }


    public class Shop : BaseViewModel
    {
        public Shop()
        {
            Store =new List<ShopViewModel>();
        }

        public string ErrorMessage { get; set; } = "";
        public List<ShopViewModel> Store { get; set; }
        public int? TotalStores { get; set; }

        public int PageNo { get; set; }
        public string CategoryType { get; set; }
        public string FilterType { get; set; }

    }
    public class ShopViewModel 
    {

       
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
        
        public string BusinessName { get; set; }
        
        public string BusinessType { get; set; }

        public string Email { get; set; }

        public short ZipCode { get; set; }

        public double AverageRating { get; set; }

        public float? MinOrderPrice { get; set; }

        public string Phone { get; set; }

        public string ImageUrl { get; set; }

        public decimal? Longitude { get; set; }

        public decimal? Latitude { get; set; }

        public DateTime? Open_From { get; set; }

        public DateTime? Open_To { get; set; }

        public string MinDeliveryTime { get; set; }

        public decimal MinDeliveryCharges { get; set; }


       
        public double Distance { get; set; }

        public IEnumerable<StoreTags> StoreTags { get; set; }
        public DeliveryHours StoreDeliveryHours { get; set; }



    }
    public class StoreTags
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public int Store_Id { get; set; }
    }

    public class DeliveryHours
    {
        public int Id { get; set; }
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


    public class StoreViewModel
    {


        public int Id { get; set; }



        public string BusinessType { get; set; }

        public string Description { get; set; }


        public string BusinessName { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }

        public double AverageRating { get; set; }


        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public float? MinOrderPrice { get; set; }





    }
}