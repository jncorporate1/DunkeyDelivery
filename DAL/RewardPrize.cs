using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class RewardPrize
    {
        public RewardPrize()
        {
            RewardMilestones = new HashSet<RewardMilestones>();
        }
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }

        [JsonIgnore]
        public virtual ICollection<RewardMilestones> RewardMilestones { get; set; }
    }
}
