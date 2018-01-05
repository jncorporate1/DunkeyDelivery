using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class CreditCard
    {
        public int Id { get; set; }

        [Required]
        public string CCNo { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        [Required]
        public string CCV { get; set; }

        public int? Is_Primary { get; set; }

        public string Label { get; set; }

        public Boolean is_delete { get; set; }

        [Required]
        public string BillingCode { get; set; }

        public int User_ID { get; set; }

        public virtual User User { get; set; }
    }
}
