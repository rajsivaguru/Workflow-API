using System;
using System.Collections.Generic;

namespace StaffingService.Models
{
    public class Period
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }     
    }

    public class JobReportParam
    {
        public string referenceid { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string publisheddate { get; set; }
        public short status { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int lastdays { get; set; }
        public string reporttype { get; set; }
        public int loginid { get; set; }
    }

    public class JobReport
    {
        public string referenceid { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string publisheddate { get; set; }
        public string clientname { get; set; }
        public string isactive { get; set; }
        public int usercount { get; set; }
        public string users { get; set; }
        public string duration { get; set; }
        public int submission { get; set; }
        public string outgoingcall { get; set; }
    }

    public class UserReportParam
    {
        public List<int> userids { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string publisheddate { get; set; }
        public string assingeddate { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int lastdays { get; set; }
        public string reporttype { get; set; }
    }

    public class ClientReportParam
    {
        public List<int> clientids { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string publisheddate { get; set; }
        public int lastdays { get; set; }
        public string reporttype { get; set; }
        public int loginid { get; set; }
    }

    public class ProfileSearchReportParam
    {
        public List<int> userids { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string searcheddate { get; set; }
        public int lastdays { get; set; }
        public string reporttype { get; set; }
    }

    public class UserReport
    {
        public int userid { get; set; }
        public string username { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string clientname { get; set; }
        public string location { get; set; }
        public string publisheddate { get; set; }
        public string assigneddate { get; set; }
        public string duration { get; set; }
        public int submission { get; set; }
        public string outgoingcall { get; set; }
        public string comment { get; set; }
        public bool jobstarted { get; set; }        
    }

    public class ClientReport
    {
        public int jobid { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string clientname { get; set; }
        public string location { get; set; }
        public string publisheddate { get; set; }
        public int submissions { get; set; }
        public int assignmentcount { get; set; }
        public bool jobstatus { get; set; }
    }

    public class ProfileSearchReport
    {
        public int recordid { get; set; }
        public string username { get; set; }
        public string jobcode { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public string headline { get; set; }
        public string skill1 { get; set; }
        public string skill2 { get; set; }
        public string skill3 { get; set; }
        public string searcheddate { get; set; }
        public string searchengine { get; set; }
        public string searchurl { get; set; }
        public bool isjobseeker { get; set; }
        public bool isoverride { get; set; }
    }

    public class PunchReportParam
    {
        public List<int> userRoleIds { get; set; }
        public string fromdate { get; set; }
        public string todate { get; set; }
        public int lastdays { get; set; }
        public bool showonlymissingtime { get; set; }
        public string reporttype { get; set; }
        public int loginid { get; set; }
    }

    public class PunchReport
    {
        public string punchdate { get; set; }
        public string username { get; set; }
        public string intime { get; set; }
        public string outtime { get; set; }
        public string notes { get; set; }
    }
}