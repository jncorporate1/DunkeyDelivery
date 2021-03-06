namespace DAL
{
    using DAL;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Store : ICloneable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Store()
        {
            Categories = new HashSet<Category>();
            Drivers = new HashSet<Driver>();
            //Orders = new HashSet<Order>();
            StoreOrders = new HashSet<StoreOrder>();
            Payment_Method = new HashSet<Payment_Method>();
            Products = new HashSet<Product>();
            StoreRatings = new HashSet<StoreRatings>();
            //  StoreDeliveryHours = new HashSet<StoreDeliveryHours>();
            Offers = new HashSet<Offer>();
            StoreTags = new HashSet<StoreTags>();
            Admins = new HashSet<Admin>();
            Packages = new HashSet<Package>();
            RatingType = new RatingTypes();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int Id { get; set; }


        [Required]
        public string BusinessType { get; set; }

        public string Description { get; set; }

        [Required]
        public string BusinessName { get; set; }

        [NotMapped]
        public double Distance { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public TimeSpan Open_From { get; set; }

        public TimeSpan Open_To { get; set; }

        [NotMapped]
        public double AverageRating { get; set; }


        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public string ContactNumber { get; set; }

        public int MinDeliveryTime { get; set; }

        public decimal? MinDeliveryCharges { get; set; }

        public float? MinOrderPrice { get; set; }

        public bool IsDeleted { get; set; }

        public DbGeography Location { get; set; }

        [NotMapped]
        public RatingTypes RatingType { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreDeliveryTypes> StoreDeliveryTypes { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Driver> Drivers { get; set; }

        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order> Orders { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Payment_Method> Payment_Method { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreRatings> StoreRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual StoreDeliveryHours StoreDeliveryHours { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer> Offers { get; set; }

        //[JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreTags> StoreTags { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Admin> Admins { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package> Packages { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrder> StoreOrders { get; set; }

        [NotMapped]
        public double BusinessTypeTax { get; set; }

        [NotMapped]
        public int CategoryType { get; set; }

        public class RatingTypes
        {
            public int FiveStar { get; set; }

            public int FourStar { get; set; }

            public int ThreeStar { get; set; }

            public int TwoStar { get; set; }

            public int OneStar { get; set; }

            public int TotalRatings { get; set; }
        }

       
    }
 
}
