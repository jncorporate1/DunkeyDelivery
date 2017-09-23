namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Payment_Details
    {
        public int Id { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public string CardNo { get; set; }

        [Required]
        public string CVV { get; set; }

        public DateTime ValidUpto { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }
    }
}
