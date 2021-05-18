using Newtonsoft.Json;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;

namespace StaffingService.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public async override void OnException(HttpActionExecutedContext context)
        {
            int errorId;

            ////switch (context.Exception.GetType().Name)
            ////{
            ////    case Constants.ExceptionType.NULLREFERENCE:
            ////        break;
            ////    case Constants.ExceptionType.SQL:
            ////        break;
            ////}

            object sourceData;
            StringBuilder fieldValue = new StringBuilder();

            /* Get fieldname and value. */
            for (int i = 0; i < context.ActionContext.ActionArguments.Count; i++)
            {
                sourceData = context.ActionContext.ActionArguments.ElementAt(i);
                var source = ((KeyValuePair<string, object>)sourceData);

                if (((KeyValuePair<string, object>)sourceData).Value != null)
                {
                    if ((((KeyValuePair<string, object>)sourceData).Value.GetType()).IsClass)
                    {
                        fieldValue.Append($"{source.Key}: {JsonConvert.SerializeObject(source.Value)};");
                    }
                    else
                    {
                        fieldValue.Append($"{source.Key}: {source.Value};");
                    }
                }
                else
                    fieldValue.Append($"{source.Key}: null;");
            }

            string fieldData = fieldValue.ToString();
            Random random = new Random();
            errorId = random.Next();

            ErrorLog error = new ErrorLog()
            {
                ErrorCode = errorId,
                Controller = context.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                Method = context.Request.RequestUri.ToString(),
                InputValue = fieldData,
                InnerException = context.Exception.InnerException == null ? null : context.Exception.InnerException.ToString(),
                Message = context.Exception.Message,
                StackTrace = context.Exception.StackTrace
            };

            int dbErrorId = await Common.LogErrorInDBAsync(error);

            ResponseModel responseObj = new ResponseModel()
            {
                ErrorCode = errorId,
                ResultStatus = Constants.ResponseResult.ERROR,
                RequestType = context.Request.Method.ToString()
            };

            if (dbErrorId > 0)
                responseObj.ErrorMessage = $"Error ID: {dbErrorId}. {Environment.NewLine}";
            else if (errorId > 0)
                responseObj.ErrorMessage = $"Error ID: {errorId}. {Environment.NewLine}";

            switch (context.Request.Method.ToString())
            {
                case "GET":
                    responseObj.ErrorMessage += "Error occurred while getting data.";
                    responseObj.RequestType = Constants.RequestType.GET;
                    break;
                case "POST":
                    responseObj.RequestType = Constants.RequestType.POST;
                    if (error.Controller.ToLower() == "email")
                        responseObj.ErrorMessage += "Error occurred while sending email.";
                    else
                        responseObj.ErrorMessage += "Error occurred while saving data.";
                    break;
                default:
                    break;
            }

            var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, responseObj);
            context.Response = response;

            base.OnException(context);
        }
    }
}