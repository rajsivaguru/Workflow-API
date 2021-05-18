using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StaffingService.Controllers
{
    [UserAuthorizationFilter]
    [CustomExceptionFilter]
    public class RecruiterController : BaseController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetRecruiterJobList(int userid = 0)
        {
            int? userId = Common.GetLoginId(Request.Headers);

            if (!userId.HasValue && userid > 0)
                userId = userid;

            ResponseModel result = await RecruiterDal.Instance.GetRecruiterJobList(Convert.ToInt32(userId));
            return SendResult(result);
        }
        
        [HttpPost]
        public async Task<HttpResponseMessage> StartRecruiterJob(int jobassignmentid)
        {
            int? loginId = Common.GetLoginId(Request.Headers);

            ResponseModel result = await RecruiterDal.Instance.StartRecruiterJob(jobassignmentid, Convert.ToInt32(loginId));
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> StopRecruiterJob(RecruitersJobs source)
        {
            int? loginId = Common.GetLoginId(Request.Headers);

            ResponseModel result = await RecruiterDal.Instance.StopRecruiterJob(source, Convert.ToInt32(loginId));
            return SendResult(result);
        }
    }
}