//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DunkeyDelivery
{
    using System;
    using System.Collections.Generic;
    
    public partial class Orders
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Orders()
        {
            this.Status = 0;
            this.Order_Items = new HashSet<Order_Items>();
            this.Driver_Orders = new HashSet<Driver_Orders>();
        }
    
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public System.DateTime DateTime { get; set; }
        public Nullable<short> Status { get; set; }
        public System.DateTime DeliveryDateTime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Items> Order_Items { get; set; }
        public virtual Users User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Driver_Orders> Driver_Orders { get; set; }
        public virtual Store Store { get; set; }
    }
}