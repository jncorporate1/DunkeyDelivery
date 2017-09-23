namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Package_Products
    {
        public int Id { get; set; }

        [Required]
        public string Qty { get; set; }

        public int Product_Id { get; set; }

        public int Package_Id { get; set; }

        public virtual Package Package { get; set; }

        public virtual Product Product { get; set; }
    }
}
