using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PharmacyRequest
    {
        public PharmacyRequest()
        {
            PharmacyRequest_Products = new HashSet<PharmacyRequest_Products>();
        }
        public int Id { get; set; }
        public string Doctor_FirstName { get; set; }
        public string Doctor_LastName { get; set; }
        public string Doctor_Phone { get; set; }
        public string Patient_FirstName { get; set; }
        public string Patient_LastName { get; set; }
        public DateTime Patient_DOB { get; set; }
        public int Gender { get; set; }
        public string Delivery_Address { get; set; }
        public string Delivery_City { get; set; }
        public string Delivery_State { get; set; }
        public string Delivery_Zip { get; set; }
        public string Delivery_Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int Status { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PharmacyRequest_Products> PharmacyRequest_Products { get; set; }
    }
}
