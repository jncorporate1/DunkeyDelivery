using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminSubAdminNotifications
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public string Title { get; set; }

        [Required]
        public int Status { get; set; }

        public int AdminId { get; set; }

        public DateTime CreatedDate { get; set; }

        public virtual Admin Admin { get; set; }
        
        public int AdminNotification_Id { get; set; }

        public virtual AdminNotifications AdminNotification { get; set; }
    }
}
