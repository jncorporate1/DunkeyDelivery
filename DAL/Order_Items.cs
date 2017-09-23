namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order_Items
    {
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int Price { get; set; }

        public int Order_Id { get; set; }

        public int Product_Id { get; set; }

        public int Package_Id { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }

        public virtual Package Package { get; set; }
        
    }
}
