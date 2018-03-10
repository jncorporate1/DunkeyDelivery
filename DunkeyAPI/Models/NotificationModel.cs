using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Models
{
    public class NotificationModel
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public int NotificationId { get; set; }

        public int Type { get; set; }

        public string EntityId { get; set; }
    }
}