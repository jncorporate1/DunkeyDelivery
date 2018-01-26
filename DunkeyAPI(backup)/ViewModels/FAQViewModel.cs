using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{
    public class FAQViewModel
    {

        public int Id { get; set; }
        [Required]
        public string Question { get; set; }
        [Required]
        public string Answer { get; set; }
        [Required]
        public string Type { get; set; }
    }

}