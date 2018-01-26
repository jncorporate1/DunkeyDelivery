using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.BindingModels
{
    public class FAQBindingModel: BaseViewModel 
    {

        public int Id { get; set; }
        [Required(ErrorMessage ="Question is required.")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Answer is required.")]
        public string Answer { get; set; }
        [Required]
        public string Type { get; set; }
    }
}