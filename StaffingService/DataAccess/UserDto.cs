namespace StaffingService.DataAccess
{
    public class UserDto
    {
        public int userid { get; set; }
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string minitial { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string workphone { get; set; }
        public string mobile { get; set; }
        public string homephone { get; set; }
        public string location { get; set; }
        public string imgurl { get; set; }
        public int status { get; set; }
    }
    public class UserAssignmentDto
    {
        public int userid { get; set; }
        public string email { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string minitial { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string imgurl { get; set; }
        public int jobsassigned { get; set; }
        public bool isabsent { get; set; }
    }
}