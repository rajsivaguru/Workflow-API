using Newtonsoft.Json;
using StaffingService.DataAccess;
using StaffingService.Models;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using WorkFlow.Business;

namespace WorkFlow.Controllers
{
    public class TDWController : ApiController
    {
        private ITD_WorkFlow objService = null;

        public TDWController()
        {
            objService = new TD_WorkFlow();
        }

        [HttpGet]
        public string GetConfiguration()
        {
            try
            {
                string deploymentMessage = string.Empty;
                DateTime now = DateTime.Now;
                DateTime then = Convert.ToDateTime(ConfigurationManager.AppSettings["LastDeploymentDateTime"].ToString());
                
                double totalHours = (now - then).TotalHours;

                if (totalHours <= 24)
                    deploymentMessage = ConfigurationManager.AppSettings["DeploymentNotificationMessage"].ToString();

                var jsonResult = new
                {
                    GoogleClientID = ConfigurationManager.AppSettings["GoogleClientID"].ToString(),
                    JobTimerDuration = ConfigurationManager.AppSettings["JobTimerDuration"].ToString(),
                    AlertTimerDuration = ConfigurationManager.AppSettings["AlertTimerDuration"].ToString(),
                    HelpDeskURL = ConfigurationManager.AppSettings["HelpDeskURL"].ToString(), 
                    StaffDirectoryURL = ConfigurationManager.AppSettings["StaffDirectoryURL"].ToString(),
                    DeploymentNotificationMessage = deploymentMessage
                };

                return JsonConvert.SerializeObject(jsonResult);
            }
            catch (Exception ex)
            {
                var jsonResult = new
                {
                    GoogleClientID = "",
                    JobTimerDuration = 3600,
                    AlertTimerDuration = 30,
                    HelpDeskURL = "",
                    StaffDirectoryURL = ""
                };
                return JsonConvert.SerializeObject(jsonResult);
            }
        }
        
        [HttpGet]
        public string SearchUser(string keyword)
        {
            return objService.SearchUser(keyword);
        }

        [HttpGet]
        public string SearchJob(string keyword)
        {
            return objService.SearchJob(keyword);
        }


        [HttpGet]
        public string BindUsers(string status, string loginid)
        {
            return objService.BindUsers(status,loginid);
        }

        [HttpGet]
        public string BindPriority()
        {
            return objService.BindPriority();
        }


        [HttpGet]
        public string BindRoles(string name_startsWith)
        {
            return objService.BindRoles(name_startsWith);
        }

        [HttpGet]
        public string SaveUser(string sUserModel, string loginid)
        {
            return objService.SaveUser(sUserModel, loginid);
        }
        
        [HttpGet]
        public string BindJobs(string loginid)
        {
            return objService.BindJobs(loginid);
        }
        
        [HttpGet]
        public string SaveJobUser(string userid, string jobid, string priorityid, string clientname, string loginid)
        {
            return objService.SaveJobUser(userid, jobid, priorityid, clientname, loginid);
        }

        [HttpGet]
        public string BindJobStatus(string jobassignmentid)
        {
            return objService.BindJobStatus(jobassignmentid);
        }

        [HttpGet]
        public string BindJobStatusHistory(string jobassignmentid)
        {
            return objService.BindJobStatusHistory(jobassignmentid);
        }
        

        [HttpGet]
        public string UpdateJobStatus(string jobassignmentid, string statusid, string comment, string userid)
        {
            return objService.UpdateJobStatus(jobassignmentid, statusid, comment, userid);
        }
        
    }
}