namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            Driver_Orders = new HashSet<Driver_Orders>();
            Order_Items = new HashSet<Order_Items>();
        }

        public int Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        public DateTime DateTime { get; set; }

        public short? Status { get; set; }

        public DateTime DeliveryDateTime { get; set; }

        public int User_Id { get; set; }

        public int Store_Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Driver_Orders> Driver_Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Items> Order_Items { get; set; }

        public virtual Store Store { get; set; }

        public virtual User User { get; set; }
    }
}
