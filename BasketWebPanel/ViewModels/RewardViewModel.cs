using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{

    public class AddRewardViewModel : BaseViewModel
    {
        public AddRewardViewModel()
        {
            Rewards = new RewardViewModel();
        }
        public RewardViewModel Rewards { get; set; }
    }

    public class RewardViewModel
    {
        public RewardViewModel()
        {
            RewardPrizes = new RewardPrize();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.Text)]
        public int PointsRequired { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int Type { get; set; }// 0 for amount 1 for Gift 

        public double AmountAward { get; set; }

        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public int? RewardPrize_Id { get; set; }

        public RewardPrize RewardPrizes { get; set; }
    }
    public class RewardPrize
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";

    }


}