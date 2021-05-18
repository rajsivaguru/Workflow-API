using System;

namespace StaffingService.Models
{
    public class ProfileSearchCriteria
    {
        public int profilesearchid { get; set; }
        public int userid { get; set; }
        public int? jobid { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string headline { get; set; }
        public string skill1 { get; set; }
        public string skill2 { get; set; }
        public string skill3 { get; set; }
        public string searchstring { get; set; }
        public string searchengine { get; set; }
        public bool isjobseeker { get; set; }
        public bool isoverride { get; set; }
        public DateTime createdon { get; set; }
    }
}