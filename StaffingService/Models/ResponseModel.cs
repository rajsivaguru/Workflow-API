using System;

namespace StaffingService.Models
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            ResultStatus = OutputCount = 0;
            SuccessMessage = ErrorMessage = string.Empty;
            RequestType = "GET";
        }

        public int ErrorCode { get; set; }
        public int ResultStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public Object Output { get; set; }
        public int OutputCount { get; set; }
        public string RequestType { get; set; }
    }
}