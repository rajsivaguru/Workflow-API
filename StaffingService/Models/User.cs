using System.Collections.Generic;

namespace StaffingService.Models
{
    public class User
    {
        public int userid { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string name { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public List<string> rolenames { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string homephone { get; set; }
        public string location { get; set; }
        public string imgurl { get; set; }
        public int status { get; set; }
        public string token { get; set; }
    }

    public class UserAssignment
    {
        public int userid { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string imgurl { get; set; }
        public int jobsassigned { get; set; }
        public bool isabsent { get; set; }
    }
}