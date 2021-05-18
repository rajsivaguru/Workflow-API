namespace StaffingService.Models
{
    public class Config
    {
        public class EmploymentType
        {
            public int employmenttypeid { get; set; }
            public string code { get; set; }
            public string description { get; set; }
            public int sortorder { get; set; }
        }

        public class CommunicationMode
        {
            public int communicationid { get; set; }
            public string mode { get; set; }
            public string description { get; set; }
            public int sortorder { get; set; }
        }

        public class BillingFrequency
        {
            public int billingfrequencyid { get; set; }
            public string frequency { get; set; }
            public string description { get; set; }
            public int sortorder { get; set; }
        }
    }
}