using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.ViewModels
{

    public class TaxListViewModel
    {
        public TaxListViewModel()
        {
            Tax = new List<BusinessTypeTax>();
        }
        public List<BusinessTypeTax> Tax { get; set; }
    }
}