using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Hubs;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WorkFlow.Business;

namespace StaffingService.Controllers
{
    [CustomExceptionFilter]
    public class JobController : ApiController
    {
        private ITD_WorkFlow _bal = null;
        private NotificationHub _hub = null;

        public JobController()
        {
            _bal = new TD_WorkFlow();
            _hub = new Hubs.NotificationHub();
        }

        [HttpGet]
        public async Task<string> SynchJobsXML()
        {
            return await _bal.SynchJobsXML();
        }
        
        //[System.Web.Mvc.Route("Notify")]
        //[HttpGet]
        //public HttpResponseMessage NotifyTest(string from, string message)
        //{
        //    NotificationHub hub = new NotificationHub();
        //    hub.NotifyAll(from, message);
        //    return Request.CreateResponse(HttpStatusCode.InternalServerError, from + " : " + message);
        //}
        
        [HttpGet]
        public async Task<HttpResponseMessage> GetJobDetails(int id, string reference)
        {
            ResponseModel result = await JobDal.Instance.GetJobDetails(id, reference);
            return sendResult(result);
        }

        [System.Web.Mvc.Route("JobDetailsByRef")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetJobDetailsByRef(string reference)
        {
            ResponseModel result = await JobDal.Instance.GetJobDetails(0, reference);
            return sendResult(result);
        }
        
        [HttpGet]
        public async Task<HttpResponseMessage> GetJobList(int loginid)
        {
            ResponseModel result = await JobDal.Instance.GetJobList(loginid);
            return sendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetJobListForDD(int loginid)
        {
            ResponseModel result = await JobDal.Instance.GetJobListForDD(loginid);
            return sendResult(result);
        }

        [UserAuthorizationFilter]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPriorityJobs(int isAll = 0)
        {
            int? loginId = Common.GetLoginId(Request.Headers);
            ResponseModel result = await JobDal.Instance.GetJobPriorityList(Convert.ToInt32(loginId), isAll);
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveJobAssignment(JobAssignment assignment)
        {
            ResponseModel result = await JobDal.Instance.SaveJobAssignment(assignment);
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveJobsAssignment(List<JobAssignment> assignments)
        {
            ResponseModel result = await JobDal.Instance.SaveJobsAssignment(assignments);
            return sendResult(result);
        }
        
        [UserAuthorizationFilter]
        [HttpGet]
        public async Task<HttpResponseMessage> SavePriorityJob(string jobIds)
        {
            int? loginId = Common.GetLoginId(Request.Headers);
            ResponseModel result = await JobDal.Instance.SavePriorityJob(jobIds, Convert.ToInt32(loginId));
            return sendResult(result);
        }

        [UserAuthorizationFilter]
        [HttpPost]
        public async Task<HttpResponseMessage> SaveInterestedJob(Job job)
        {
            int? loginId = Common.GetLoginId(Request.Headers);
            UserCache user = Common.GetUser(Request.Headers);
            
            ResponseModel result = await JobDal.Instance.SaveInterestedJob(job.jobid, Convert.ToInt32(loginId));

            if (result.ResultStatus == 1)
            {
                //ResponseModel resultNotification = await CommonDal.Instance.GetNotification(Convert.ToInt32(loginId));

                //if(resultNotification.OutputCount > 0)
                    _hub.NotifyJobInterest(job, user, Convert.ToInt32(result.Output));

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);

            ////_hub.NotifyJobInterest(job, user);
            ////return Request.CreateResponse(HttpStatusCode.OK, "SUCCESS");
        }
        

        [HttpGet]
        public HttpResponseMessage Redirection()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, true);
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        #region Google Indexing Test
        /*TODO:
         * Install this package for indexing service.
         * <package id="Google.Apis.Indexing.v3" version="1.38.0.1491" targetFramework="net452" /> 
        */
        ////[HttpGet]
        ////public async Task<HttpResponseMessage> Indexing(string file)
        ////{
        ////    try
        ////    {
        ////        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
        ////        {
        ////            //ClientId = "753784829804-d881hvgn7je19fj8pp0goe09gja7o5cp.apps.googleusercontent.com",
        ////            //ClientSecret = "w1dOQQdEbYG_NzgpjpaSxfNw"
        ////            ClientId = "753784829804-d881hvgn7je19fj8pp0goe09gja7o5cp.apps.googleusercontent.com",
        ////            ClientSecret = "w1dOQQdEbYG_NzgpjpaSxfNw"
        ////        }, new[] { IndexingService.Scope.Indexing }, "user", CancellationToken.None, new FileDataStore("IndexAPI.Google"));

        ////        //var abc = new Google.Apis.Indexing.v3.Data.UrlNotification();
        ////        //abc.Url = "https://www.apps.techdigitalcorp.com/Workflow-Dev/gjobs/qa.html";
        ////        //abc.Type = "URL_UPDATED";

        ////        var service = new IndexingService(new Google.Apis.Services.BaseClientService.Initializer
        ////        {
        ////            //ApiKey = "AIzaSyCHT7SEp5bPMxvafWIYs_QcYgIelGgFDWo",
        ////            //ApplicationName = "API key - Custom Search",
        ////            HttpClientInitializer = credential
        ////        });

        ////        //var service = new IndexingService();

        ////        if(string.IsNullOrWhiteSpace(file))
        ////        {
        ////            var pub = service.UrlNotifications.Publish(new Google.Apis.Indexing.v3.Data.UrlNotification
        ////            {
        ////                Url = "https://www.apps.techdigitalcorp.com/Workflow-Dev/gjobs/java.html",
        ////                Type = "URL_UPDATED",
        ////                NotifyTime = System.DateTime.Now
        ////            });
        ////        }
        ////        else
        ////        {
        ////            var pub = service.UrlNotifications.Publish(new Google.Apis.Indexing.v3.Data.UrlNotification
        ////            {
        ////                Url = $"https://www.apps.techdigitalcorp.com/Workflow-Dev/gjobs/{file}.html",// "https://www.apps.techdigitalcorp.com/Workflow-Dev/gjobs/" + file + ".html",
        ////                Type = "URL_UPDATED",
        ////                NotifyTime = System.DateTime.Now
        ////            });
        ////        }

        ////        ////var pub2 = service.UrlNotifications.Publish(new Google.Apis.Indexing.v3.Data.UrlNotification
        ////        ////{
        ////        ////    Url = "https://www.apps.techdigitalcorp.com/Workflow-Dev/gjobs/data2.html",
        ////        ////    Type = "URL_DELETED",
        ////        ////    NotifyTime = System.DateTime.Now
        ////        ////});

        ////        var metas = service.UrlNotifications.GetMetadata();

        ////        return Request.CreateResponse(HttpStatusCode.OK, credential);
        ////    }
        ////    catch(System.Exception ex)
        ////    {
        ////        return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        ////    }            
        ////}

        #endregion

        private HttpResponseMessage sendResult(ResponseModel result)
        { 
            if (result.ResultStatus == 1)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        }


        //[HttpGet]
        ////public async Task<string> AssignJobUser(JobAssignment assignment)
        //public async Task<string> AssignJobUser(string source)
        //{
        //    JobAssignment assignment = JsonConvert.DeserializeObject<JobAssignment>(source);
        //    return await JobDal.Instance.AssignJobUsers(assignment);
        //}
    }
}
