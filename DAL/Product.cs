namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
          //  Order_Items = new HashSet<Order_Items>();
            Offer_Products = new HashSet<Offer_Products>();
            Package_Products = new HashSet<Package_Products>();
            Favourites = new HashSet<Favourite>();
            PharmacyRequest_Products = new HashSet<PharmacyRequest_Products>();
            ProductSizes = new HashSet<ProductSizes>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public double Price { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public string Image_Selected { get; set; } = "";
        
        public short Status { get; set; }

        public bool IsDeleted { get; set; }

        public int? Category_Id { get; set; } = 0; // was optional

        public int Store_Id { get; set; }

        public string Size { get; set; }

        [NotMapped]
        public string BusinessName { get; set; } = "";

        [NotMapped]
        public int MinDeliveryTime { get; set; } = 0;

        [NotMapped]
        public string BusinessType { get; set; } = "";

        [NotMapped]
        public decimal? MinDeliveryCharges { get; set; } = -1;

        [NotMapped]
        public float? MinOrderPrice { get; set; } = -1;

        public int ItemId { get; set; } = 0;

        [JsonIgnore]
        public virtual Category Category { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }

        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order_Items> Order_Items { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer_Products> Offer_Products { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package_Products> Package_Products { get; set; }


        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favourite> Favourites { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PharmacyRequest_Products> PharmacyRequest_Products { get; set; }


        public virtual ICollection<ProductSizes> ProductSizes { get; set; }

        public virtual Store Store { get; set; }
    }
}
