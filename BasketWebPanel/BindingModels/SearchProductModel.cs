using BasketWebPanel.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BasketWebPanel.BindingModels
{
    public class SearchProductModel : BaseViewModel
    {
        public string ProductName { get; set; } = "";
        public float? ProductPrice { get; set; }
        public string CategoryName { get; set; } = "";
        public new int? StoreId { get; set; }
        public int? PackageId { get; set; }
        public SelectList StoreOptions { get; internal set; }
    }

    public class SearchOrderModel : BaseViewModel
    {
        public string Id { get; set; } = "";

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate { get; set; }

        public new int? StoreId { get; set; }
        public int? OrderStatusId { get; set; }
        public int? PaymentStatusId { get; set; }
        public int? PaymentMethodId { get; set; }
        public SelectList StoreOptions { get; internal set; }
        public SelectList OrderStatusOptions { get; internal set; }
        public SelectList PaymentStatusOptions { get; internal set; }
        public SelectList PaymentMethodOptions { get; internal set; }
    }

    public class SearchCategoryModel : BaseViewModel
    {
        public string CategoryName { get; set; } = "";
        public new int? StoreId { get; set; }
        public SelectList StoreOptions { get; internal set; }
    }

    public class SearchPackageModel : BaseViewModel
    {
        public string PackageName { get; set; } = "";
        public new int? StoreId { get; set; }
        public SelectList StoreOptions { get; internal set; }
    }

    public class SearchOfferModel : BaseViewModel
    {
        public string OfferName { get; set; } = "";
        public new int? StoreId { get; set; }
        public SelectList StoreOptions { get; internal set; }
    }

    public class SearchStoreModel
    {
        public string StoreName { get; set; } = "";
        public int? StoreId { get; set; }
        public SelectList StoreOptions { get; internal set; }
    }

    public class SearchAdminModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public int? StoreId { get; set; }

        public SelectList StoreOptions { get; internal set; }
    }
    public class SearchFAQModel
    {
        public string Type { get; set; } = "";
    }

    public class SearchBlogModel
    {
        public string BlogTitle { get; set; } = "";
        public string CategoryType { get; set; } = "";
        public string DateOfPosting { get; set; }
        public int? BlogId { get; set; }

        
    }

}