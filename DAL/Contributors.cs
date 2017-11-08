using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class Contributors
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public short? is_partner { get; set; } = 0;

        public short? is_invester { get; set; } = 0;

        public short? is_press { get; set; } = 0;

        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public short is_deleted { get; set; }

        [NotMapped]
        public string Type { get; set; }

       
    }
}
