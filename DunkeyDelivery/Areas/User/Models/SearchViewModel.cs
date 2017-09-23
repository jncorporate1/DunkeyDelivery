using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyDelivery.Areas.User.Models
{
    public class SearchViewModel
    {

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Search Name")]
        public string Search { get; set; }

        public int ObjectPerPAge { get; set; } = 6;

        public int PageNumber { get; set; } = 0;


    }
}