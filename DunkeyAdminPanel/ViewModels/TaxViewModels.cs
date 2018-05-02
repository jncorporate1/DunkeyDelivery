using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketWebPanel.ViewModels
{
    public class TaxViewModels
    {
        public int Id { get; set; }

        public string BusinessType { get; set; }

        public double Tax { get; set; }
    }

    public class ListTaxViewModel : BaseViewModel
    {
        public ListTaxViewModel()
        {
            TaxList = new List<TaxViewModels>();
        }
        public List<TaxViewModels> TaxList{get;set; }
    }

//    public class InsertTaxViewModel
//    {
//        public List<TaxId> ListTaxId { get; set; }

//        public List<ListBusinessType> ListBusinessType  { get; set; }

//        public List<ListTaxValues> ListTaxValues { get; set; }

//    }
//    public class TaxId
//    {
//        public int Id { get; set; }
//    }
//    public class ListBusinessType
//    {
//        public string BusinessType { get; set; }

//    }
//    public class ListTaxValues
//    {
//        public double Tax { get; set; }

//    }
}