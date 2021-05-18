using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingService.Models
{
    public class Notification
    {
        public int notificationid { get; set; }
        public string notificationtype { get; set; }
        public string fromuser { get; set; }
        public int? jobid { get; set; }
        public string referenceid { get; set; }
        public string title { get; set; }
        public string messagetext { get; set; }
        public bool isread { get; set; }
        public string createdon { get; set; }
    }
}