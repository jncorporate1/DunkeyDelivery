using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class Offer_Packages
    {
        public int Id { get; set; }

        public int Offer_Id { get; set; }

        public string Name { get; set; }

        public int Package_Id { get; set; }

        public DateTime ValidUpto { get; set; }

        public int DiscountedPrice { get; set; }

        public int DiscountPercentage { get; set; }

        public int SlashPrice { get; set; }

        public string ImageUrl { get; set; }

        public virtual Offer Offer { get; set; }

        public virtual Package Package { get; set; }
    }
}
