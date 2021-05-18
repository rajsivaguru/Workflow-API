using System;

namespace StaffingService.Models
{
    public class ErrorLog
    {
        public int ErrorCode { get; set; }
        public int ErrorLogId { get; set; }
        public string Controller { get; set; }
        public string Method { get; set; }
        public string InputValue { get; set; }
        public string InnerException { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
    }
}