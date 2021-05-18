using System;
using System.Collections.Generic;

namespace StaffingService.Models
{
    public class Job
    {
        public int jobid { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string formattedtitle { get; set; }
        public string formattedtitle2 { get; set; }
        public string duration { get; set; }
        public string description { get; set; }
        public string clientname { get; set; }
        public string publisheddate { get; set; }
        public string startdate { get; set; }
        public string referenceid { get; set; }
        public int priorityLevel { get; set; }
        public int userlist { get; set; }
        public string selectedUser { get; set; }
        public bool isactive { get; set; }
    }

    public class JobFormatted
    {
        public int jobid { get; set; }
        public int rowid { get; set; }
        public string formattedtitle { get; set; }
        public string title { get; set; }
        public string location { get; set; }
    }

    public class JobAssignment
    {
        public int jobid { get; set; }
        public string clientname { get; set; }
        public int priorityid { get; set; }
        public List<int> userids { get; set; }
        public int loginid { get; set; }
    }

    public class PriorityJob
    {
        public int jobid { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string clientname { get; set; }
        public string publisheddate { get; set; }
        public string prioritizeddate { get; set; }
        public string referenceid { get; set; }
        public bool isprioritized { get; set; }
        public bool isinterested { get; set; }
        public string assigneduser { get; set; }
    }

    /* For Job publish/Indexing */
    public class JobDetail
    {
        public int JobId { get; set; }
        public string ReferenceId { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Duration { get; set; }
        public string Description { get; set; }
        public string ClientName { get; set; }
        public DateTime Publisheddate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool JobStatus { get; set; }
        public decimal MinRate { get; set; }
        public decimal MaxRate { get; set; }
    }
}