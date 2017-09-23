using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class Rider
    {

        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string BusinessName { get; set; }

        public string BusinessType { get; set; }

        [Required]
        public string Email { get; set; }

        public string ZipCode { get; set; }   
        
        public short? Status { get; set; } 

        [Required]
        public string Phone { get; set; }

        public short SignInType { get; set; }
    }
}
