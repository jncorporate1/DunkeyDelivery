namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Store_Timings
    {
        public int Id { get; set; }

        [Required]
        public string Shift { get; set; }

        [Required]
        public string Timing { get; set; }

        public int Store_Id { get; set; }

        public virtual Store Store { get; set; }
    }
}
