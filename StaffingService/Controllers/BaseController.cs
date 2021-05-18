using StaffingService.Models;
using StaffingService.Util;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace StaffingService.Controllers
{
    public class BaseController : ApiController
    {
        protected override void Initialize(HttpControllerContext context)
        {
            base.Initialize(context);
        }
        
        internal HttpResponseMessage SendResult(ResponseModel result)
        {
            if (result == null)
            {
                result = new ResponseModel()
                {
                    ErrorMessage = Constants.ErrorMessage.MISSINGDATA,
                    SuccessMessage = string.Empty,
                    ResultStatus = Constants.ResponseResult.MISSINGDATA
                };
                return Request.CreateResponse(HttpStatusCode.BadRequest, result);
            }
            else if (result.ResultStatus == Constants.ResponseResult.SUCCESS || result.ResultStatus == Constants.ResponseResult.NODATA)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        }
        
    }
}