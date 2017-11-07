using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    /// <summary>
    /// This class is used for storing notification that was sent to users or admins. Same notification will also be inserted in notification against each user(if target audience is user) and in adminsubadminnotifications against each admin/subadmin if target audience is admins. 
    /// </summary>
    public class AdminNotifications
    {
        public AdminNotifications()
        {
            Notifications = new HashSet<Notification>();
            AdminSubAdminNotifications = new HashSet<AdminSubAdminNotifications>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int TargetAudienceType { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> Notifications { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminSubAdminNotifications> AdminSubAdminNotifications { get; set; }
    }
}
