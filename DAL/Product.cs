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
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Price { get; set; }

        public string Description { get; set; }

        public string Image { get; set; }

        public short Status { get; set; }

        public bool IsDeleted { get; set; }

        public int? Category_Id { get; set; }

        public int Store_Id { get; set; }

        public string Size { get; set; }

        public virtual Category Category { get; set; }

        //[JsonIgnore]
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order_Items> Order_Items { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Offer_Products> Offer_Products { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Package_Products> Package_Products { get; set; }

        public virtual Store Store { get; set; }
    }
}
