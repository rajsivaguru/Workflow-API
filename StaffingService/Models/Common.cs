namespace StaffingService.Models
{
    public class Status
    {
        public int statusid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int sortorder { get; set; }
    }

    public class CustomerType
    {
        public int customertypeid { get; set; }
        public string name { get; set; }
    }

    public class CustomerVendor
    {
        public int customervendorid { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}