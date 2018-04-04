using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
   public partial class SizesUnits
    {
         public int Id { get; set; }

        public string Unit { get; set; }

        public string BusinessType { get; set; }

        public bool IsDeleted { get; set; }

    }
}
