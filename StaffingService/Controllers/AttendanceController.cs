using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StaffingService.Controllers
{
    [UserAuthorizationFilter]
    [CustomExceptionFilter]
    public class AttendanceController : BaseController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetMyPunchDetails()
        {
            ResponseModel result = null;
            int? loginId = Common.GetLoginId(Request.Headers);
            
            result = await AttendanceDal.Instance.GetMyPunchDetails(Convert.ToInt32(loginId));
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveMyPunchDetails(PunchInOut source)
        {
            ResponseModel result = null;
            int? loginId = Common.GetLoginId(Request.Headers);
            result = await AttendanceDal.Instance.SaveMyPunchDetails(source, Convert.ToInt32(loginId));
            return SendResult(result);
        }
    }
}