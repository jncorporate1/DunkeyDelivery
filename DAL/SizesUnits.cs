using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
   public partial class SizesUnits
    {
        public SizesUnits()
        {
            ProductSizes = new HashSet<ProductSizes>();
        }

        public int Id { get; set; }

        public string Unit { get; set; }

        public bool IsDeleted { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductSizes> ProductSizes { get; set; }


    }
}
