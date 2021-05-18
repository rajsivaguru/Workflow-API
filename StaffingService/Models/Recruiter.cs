using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingService.Models
{
    public class Recruiter
    {

    }

    public class RecruitersJobs
    {
        public int jobassignmentid { get; set; }
        public int jobid { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string duration { get; set; }
        public string description { get; set; }
        public string publisheddate { get; set; }
        public string expirydate { get; set; }
        public string referenceid { get; set; }
        public int priorityid { get; set; }
        public string priorityLevel { get; set; }
        public int? jobassignmentstatusid { get; set; }
        public int submission { get; set; }
        public int notesadded { get; set; }
        public int qualificationadded { get; set; }
        public string status { get; set; }
        public bool isactive { get; set; }
        public string comment { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public string createdby { get; set; }
        public string createdon { get; set; }

    }
}