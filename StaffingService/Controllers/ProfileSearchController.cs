using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace StaffingService.Controllers
{
    [CustomExceptionFilter]
    [RoutePrefix("ProfileSearch")]
    public class ProfileSearchController: ApiController
    {
        public ProfileSearchController()
        {
        }
                
        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SaveProfileSearch(ProfileSearchCriteria source)
        {
            ResponseModel result = await ProfileSearchDal.Instance.SaveProfileSearch(source);
            return sendResult(result);
        }


        private HttpResponseMessage sendResult(ResponseModel result)
        {
            if (result.ResultStatus == 1)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        }
    }
}