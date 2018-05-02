using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{

    public class RewardListViewModel : BaseViewModel
    {
        public RewardListViewModel()
        {
            Rewards = new List<SearchRewardViewModel>();
        }
        public List<SearchRewardViewModel> Rewards = new List<SearchRewardViewModel>();
    }
    public class SearchRewardViewModel
    {
        public SearchRewardViewModel()
        {
            RewardPrizes = new RewardPrizeModel();
        } 
        public int Id { get; set; }

        public double PointsRequired { get; set; }

        public double AmountAward { get; set; }

        public string Description { get; set; }
        
        public int? RewardPrize_Id { get; set; }

        public bool IsDeleted { get; set; }

        public RewardPrizeModel RewardPrizes { get; set; }

    }
    public class RewardPrizeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }
    }

}