using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class RewardMilestones
    {
        //public RewardMilestones()
        //{
        //    //RewardPrize = new RewardPrize();
        //}
        public int Id { get; set; }

        public double PointsRequired { get; set; }

        public double AmountAward { get; set; }

        public string Description { get; set; }


        public int? RewardPrize_Id { get; set; }


        public virtual RewardPrize RewardPrize { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserRewards> UserRewards { get; set; }
        
    }
}
