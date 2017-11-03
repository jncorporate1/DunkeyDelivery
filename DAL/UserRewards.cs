using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
     public partial class UserRewards
    {
        public int Id { get; set; }

        public DateTime Created_Date { get; set; }

        public short is_deleted { get; set; }

        public int User_Id { get; set; }

        public int RewardMilestones_Id { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual RewardMilestones RewardMilestones { get; set; }
    }
}
