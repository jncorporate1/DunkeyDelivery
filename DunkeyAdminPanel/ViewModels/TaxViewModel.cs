using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class TaxViewModel
    {
        public int Id { get; set; }

        public string BusinessType { get; set; }

        [Required]
        public double Tax { get; set; }

    }

    public class TaxListViewModel : BaseViewModel
    {
        public TaxListViewModel()
        {
            Tax = new List<TaxViewModel>();
        }
        public List<TaxViewModel> Tax { get; set; }
    }
}