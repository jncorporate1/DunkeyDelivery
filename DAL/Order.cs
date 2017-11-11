namespace DAL
{
    using Newtonsoft.Json;
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
            StoreOrders = new HashSet<StoreOrder>();
        }
        public int Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime OrderDateTime { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_From { get; set; }

        [JsonConverter(typeof(JsonCustomDateTimeConverter))]
        public DateTime? DeliveryTime_To { get; set; }
        
        public int PaymentMethod { get; set; }

        public double Subtotal { get; set; }

        public double ServiceFee { get; set; }

        public double DeliveryFee { get; set; }

        public double Total { get; set; }

        public double TotalTaxDeducted { get; set; }

        public double TipAmount { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public int? OrderPayment_Id { get; set; }

        public short PaymentStatus { get; set; }
        
        public virtual OrderPayment OrderPayment { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StoreOrder> StoreOrders { get; set; }

      //  public virtual DeliveryMan DeliveryMan { get; set; }

        public int? DeliveryMan_Id { get; set; }
        public bool RemoveFromDelivererHistory { get; set; }
        public string DeliveryDetails_FirstName { get; set; }
        public string DeliveryDetails_LastName { get; set; }
        public string DeliveryDetails_Phone { get; set; }
        public string DeliveryDetails_ZipCode { get; set; }
        public string DeliveryDetails_Email { get; set; }
        public string DeliveryDetails_City { get; set; }
        public string DeliveryDetails_Address { get; set; }
        public string DeliveryDetails_AddtionalNote { get; set; }
    }
}
