using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class ProductSizes
    {
        public int Id { get; set; }

        public string Unit { get; set; }

        public double Weight { get; set; }

        public string Size { get; set; }

        public string NetWeight { get; set; }

        public short TypeID { get; set; } // 0 For Wine & Liquor & 1 For Beer

        public double Price { get; set; }

        public bool IsDeleted { get; set; }

        public int Product_Id { get; set; }
        
        public virtual Product Product { get; set; }

        public int? SizesUnit_Id { get; set; }
        
        public virtual SizesUnits SizesUnits { get; set; }

    }
}
