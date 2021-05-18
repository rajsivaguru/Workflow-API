using StaffingService.Models;
using StaffingService.Util;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace StaffingService.Filters
{
    /* Only users registered to this API are allowed to access the controller/method. */
    public class UserAuthorizationFilter : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext context)
        {
            if(!context.Request.Headers.Referrer.ToString().Contains("swagger"))
            {
                string uiToken = Common.GetUIToken(context.Request.Headers);

                if (!Common.IsUITokenValid(uiToken))
                    response(context, HttpStatusCode.Unauthorized);
            }

            base.OnAuthorization(context);
        }

        private void response(HttpActionContext context, HttpStatusCode status)
        {
            ResponseModel responseObj = new ResponseModel();
            HttpResponseMessage response = new HttpResponseMessage();

            responseObj.RequestType = context.Request.Method.Method;

            switch (status)
            {
                case HttpStatusCode.Forbidden:
                    responseObj.ResultStatus = Constants.ResponseResult.MISSINGDATA;
                    responseObj.ErrorMessage = Constants.ErrorMessage.MISSINGDATA;
                    response = context.Request.CreateResponse(HttpStatusCode.BadRequest, responseObj);
                    break;
                case HttpStatusCode.Unauthorized:
                    responseObj.ResultStatus = Constants.ResponseResult.UNAUTHORIZED;
                    responseObj.ErrorMessage = Constants.ErrorMessage.UNAUTHORIZED;
                    response = context.Request.CreateResponse(HttpStatusCode.Unauthorized, responseObj);
                    break;
            }

            context.Response = response;
            return;
        }
    }
}