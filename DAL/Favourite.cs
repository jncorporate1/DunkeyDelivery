using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class Favourite
    {
        public int Id { get; set; }

        public int Product_Id { get; set; }

        public int User_ID { get; set; }

        public bool IsDeleted { get; set; }

        public virtual Product Product { get; set; }

        public virtual User User { get; set; }
    }
}
