using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class RewardViewModel
    {
        public RewardViewModel()
        {
            UserPoints = new UserPoints();
            Rewards = new List<RewardMilestones>();
        }
        public UserPoints UserPoints { get; set; }
        public  List<RewardMilestones> Rewards { get; set; }

    }
    public class UserPoints
    {
        public double RewardPoints { get; set; } = 0;
    }
    public class UserReward
    {
        public double RewardAmount { get; set; } = 0;
    }


    public class Rewards
    {
        public List<RewardMilestones> RewardsList { get; set; }
    }



}