namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Driver_Orders
    {
        public int Id { get; set; }

        public DateTime? DeliveryTime { get; set; }

        public int Driver_Id { get; set; }

        //public int Order_Id { get; set; }

        public virtual Driver Driver { get; set; }

        //public virtual Order Order { get; set; }
    }
}
