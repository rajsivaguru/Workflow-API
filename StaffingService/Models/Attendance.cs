using System;

namespace StaffingService.Models
{
    public class Attendance
    {
    }

    public class PunchInOut
    {
        public int punchid { get; set; }
        public DateTime punchday { get; set; }
        public int userid { get; set; }
        public DateTimeOffset? intime { get; set; }
        public DateTimeOffset? outtime { get; set; }
        public string notes { get; set; }
        public bool istoday { get; set; }
        public DateTime modifiedon { get; set; }
    }
}