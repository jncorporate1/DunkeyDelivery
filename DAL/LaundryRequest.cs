using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class LaundryRequest
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public short Status { get; set; }

        public DateTime RequestDate { get; set; }

        public Boolean isDeleted { get; set; }

        public string Note { get; set; }

        public double? Weight { get; set; }
        
        public int Store_Id { get; set; }

        public int User_Id { get; set; }

        public virtual Store Store { get; set; }

        public virtual User User { get; set; }
    }
}
