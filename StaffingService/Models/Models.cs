using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace WorkFlow.Models
{
    public class UserModel
    {
        public int  userid {get;set;}        
        public string  fname {get;set;}
        public string  lname {get;set;}
        public string  minitial {get;set;}
        public string  email {get;set;}
        public int  roleid {get;set;}        
        public string  workphone {get;set;}
        public string  mobile {get;set;}
        public string  homephone {get;set;}
        public string  location {get;set;}
        public int  status {get;set;}
    }

    public class JobModel
    {
        public int jobassignmentid { get; set; }
        public int jobid { get; set; }
        public string title {get; set;}
        public string publisheddate { get; set; }
        public int userid { get; set; }
        public string name { get; set; }
        public int priorityid { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string description { get; set; }
        public string expirydate { get; set; }
        public int isactive { get; set; }
    }

    /// <summary>
    /// Used for Synchronizing jobs
    /// </summary>
    public class Job
    {
        public string JobCode { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RateMin { get; set; }
        public string RateMax { get; set; }
        public int IsActive { get; set; }
    }


    public static class ExtnMethods
    {
        static CultureInfo provider = CultureInfo.InvariantCulture;
        //to format the date as per psenergy format
        //DateTime.Now.ToString("dd/MM/yyyy hh:mm tt", provider)
        public static string ToPSEDateTime(this DateTime dateTime)
        {
            if (dateTime != null)
                return dateTime.ToString("MM-dd-yyyy hh:mm tt", provider);

            return "";
        }

        public static string ToPSEDate(this DateTime dateTime)
        {
            if (dateTime != null)
                return dateTime.ToString("MM-dd-yyyy", provider);

            return "";
        }
        public static string ToPSEDate(this DateTime? dateTime)
        {
            if (dateTime != null && dateTime.HasValue)
                return dateTime.Value.ToString("MM-dd-yyyy hh:mm tt", provider);

            return "";
        }

        public static string ToPSEDecimal(this decimal value)
        {
            if (value != null)
                return value.ToString("0.00");

            return "0.00";
        }
    }
}