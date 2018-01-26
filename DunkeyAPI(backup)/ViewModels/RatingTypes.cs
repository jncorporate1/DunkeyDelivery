using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class RatingTypes
    {
        public int FiveStar { get; set; }

        public int FourStar { get; set; }

        public int ThreeStar { get; set; }

        public int TwoStar { get; set; }

        public int OneStar { get; set; }

        public int AverageRating { get; set; }

        public int TotalRatings { get; set; }
    }
}