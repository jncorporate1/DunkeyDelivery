namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Store_Rating
    {
        public int Id { get; set; }

        public short Rating { get; set; }

        public string Feedback { get; set; }

        public int Store_Id { get; set; }

        public virtual Store Store { get; set; }
    }
}
