namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Text { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int Status { get; set; }

        public int AdminNotification_Id { get; set; }
    }
}
