using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class Admin
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string FullName { get; set; }

   
        public string BusinessName { get; set; }


        public string BusinessType { get; set; }

     
        public string ZipCode { get; set; }


        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required]
        public short Role { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string AccountNo { get; set; }

        public int? Store_Id { get; set; }

        public short? Status { get; set; }

        public virtual Store Store { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public bool ImageDeletedOnEdit { get; set; }
        public string ImageUrl { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogPosts> BlogPosts { get; set; }

    }
}
