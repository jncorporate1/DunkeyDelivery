using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserAddress
    {
        public int Id { get; set; }

        public int User_ID { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string FullAddress { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsPrimary { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
