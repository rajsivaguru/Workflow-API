using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingService.Models
{
    public class EmailDetails
    {
        public int emailtypeid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<ToEmailDetails> todetails { get; set; }
    }

    public class ComposeEmailParam
    {
        public int jobid { get; set; }
        public int userid { get; set; }
        public bool usedefaultfromaddress { get; set; }
        public bool sendseperateemail { get; set; }
        public string fromaddress { get; set; }
        public List<string> toaddresses { get; set; }
        public List<string> ccaddresses { get; set; }
        public List<string> bccaddresses { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
    }

    public class EmailType
    {
        public int EmailTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ToEmailDetails
    {
        public int emaildetailsid { get; set; }
        public int emailtypeid { get; set; }
        public string toaddress { get; set; }
    }

    public class EmailConfigDetails
    {   
        public string Server { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string AuthenticateUser { get; set; }
        public string AuthenticatePassword { get; set; }
        public string DefaultFromAddress { get; set; }
    }
}