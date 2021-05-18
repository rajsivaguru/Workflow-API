using Newtonsoft.Json;
using StaffingService.DataAccess;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using WorkFlow.DataAccess;
using WorkFlow.Models;

namespace WorkFlow.Business
{
    public class TD_WorkFlow : ITD_WorkFlow
    {

        public TD_WorkFlow()
        {
            
        }

        #region ITD_WorkFlow Members
        
        public string SearchUser(string keyword)
        {
            try
            {
                DAL objDal = new DAL();

                object[][] objParam = new object[][]
                {
                    new object[]{"@keyword",keyword}
                };


                DataSet dsResult = objDal.ExecuteDS("spSearchUsers", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string SearchJob(string keyword)
        {
            try
            {
                DAL objDal = new DAL();
                object[][] objParam = new object[][]
                {
                    new object[]{"@keyword",keyword}
                };

                DataSet dsResult = objDal.ExecuteDS("spSearchJobs", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string BindUsers(string status, string loginid)
        {
            try
            {
                DAL objDal = new DAL();
                object[][] objParam = new object[][]
                {
                    new object[]{"@status",status},
                    new object[]{"@UserId",loginid}
                    
                };
                DataSet dsResult = objDal.ExecuteDS("spGetUsers", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string BindPriority()
        {
            try
            {
                DAL objDal = new DAL();
                DataSet dsResult = objDal.ExecuteDS("exec spGetPriority");

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string BindRoles(string sSearch)
        {
            try
            {
                DAL objDal = new DAL();
                DataSet dsResult = objDal.ExecuteDS("exec spGetRoles");

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string SaveUser(string sUserModel, string loginid)
        {
            var jsonResult = new
            {
                Result = "0",
                Message = "Your user not saved, Please try again."
            };

            try
            {
                UserModel objUser = JsonConvert.DeserializeObject<UserModel>(sUserModel);

                DAL objDal = new DAL();
                object[][] objParam = new object[][]
                {
                    new object[]{"@userid", objUser.userid},
                    new object[]{"@fname", objUser.fname},
                    new object[]{"@lname",objUser.lname},
                    new object[]{"@email",objUser.email},
                    new object[]{"@minitial",objUser.minitial},
                    new object[]{"@status",objUser.status},
                    new object[]{"@location",objUser.location},
                    new object[]{"@mobile",objUser.mobile},
                    new object[]{"@workphone",objUser.workphone},
                    new object[]{"@homephone",objUser.homephone},
                    new object[]{"@roleid",objUser.roleid},

                    new object[]{"@loginid",loginid}
                };
    
                //making the order entry
                DataSet dsResult = objDal.ExecuteDS("spMaintainUsers", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    jsonResult = new
                    {
                        Result = dsResult.Tables[0].Rows[0][0].ToString(),
                        Message = dsResult.Tables[0].Rows[0][1].ToString()
                    };

                }
            }
            catch (Exception Ex)
            {

            }
            return JsonConvert.SerializeObject(jsonResult);
        }
        
        public string BindJobs(string loginid)
        {
            try
            {
                DAL objDal = new DAL();

                object[][] objParam = new object[][]
                {
                    new object[]{"@UserId",loginid}
                    
                };

                DataSet dsResult = objDal.ExecuteDS("spBindJobs", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }
        
        public string SaveJobUser(string userid, string jobid, string priorityid, string clientname, string loginid)
        {
            var jsonResult = new
            {
                Result = "0",
                Message = "Your Job not Assigned, Please try again."
            };

            try
            {
                DAL objDal = new DAL();
                object[][] objParam = new object[][]
                {
                    new object[]{"@jobid", jobid},
                    new object[]{"@userid",userid},
                    new object[]{"@priorityid",priorityid},
                    new object[]{"@clientname", clientname},
                    new object[]{"@loginid",loginid}
                };
                
                DataSet dsResult = objDal.ExecuteDS("spSaveJobUsers", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                {
                    string res = dsResult.Tables[0].Rows[0][0].ToString();
                    jsonResult = new
                    {
                        Result = res,
                        Message = res == "0" ? "Error occurred while assigning job.  Please try again." : "Job assigned successfully."
                    };
                }
            }
            catch (Exception Ex)
            {

            }
            return JsonConvert.SerializeObject(jsonResult);
        }
        
        public string BindJobStatus(string jobassignmentid)
        {
            try
            {
                DAL objDal = new DAL();

                object[][] objParam = new object[][]
                {
                    new object[]{"@jobassignmentid", jobassignmentid}
                };

                DataSet dsResult = objDal.ExecuteDS("spBindJobStatus", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string BindJobStatusHistory(string jobassignmentid)
        {
            try
            {
                DAL objDal = new DAL();

                object[][] objParam = new object[][]
                {
                    new object[]{"@jobassignmentid", jobassignmentid}
                };

                DataSet dsResult = objDal.ExecuteDS("spBindJobStatusHistory", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }

        public string UpdateJobStatus(string jobassignmentid, string statusid, string comment, string userid)
        {
            try
            {
                DAL objDal = new DAL();

                object[][] objParam = new object[][]
                {
                    new object[]{"@jobassignmentid", jobassignmentid},
                    new object[]{"@statusid", statusid},
                    new object[]{"@comment", comment},
                    new object[]{"@userid", userid}
                };

                DataSet dsResult = objDal.ExecuteDS("spUpdateJobStatus", objParam);

                if (dsResult != null && dsResult.Tables.Count > 0)
                    return JsonConvert.SerializeObject(dsResult.Tables[0]);
            }
            catch (Exception EX)
            {
            }
            return "";
        }


        public async Task<string> SynchJobsXML()
        {
            var jsonResult = new { Result = "1", Message = "Live jobs synched." };

            try
            {
                List<WorkFlow.Models.Job> feedJobs = GetXMLFeedJobs();                                
                var toAdd = feedJobs.Select(x => new XElement("Job", new XAttribute("JobCode", x.JobCode)
                                                        , new XAttribute("Title", x.Title ?? string.Empty)
                                                        , new XAttribute("Location", x.Location ?? string.Empty)
                                                        , new XAttribute("Description", x.Description ?? string.Empty)
                                                        , new XAttribute("PubDate", x.PubDate ?? string.Empty)
                                                        , new XAttribute("StartDate", x.StartDate ?? string.Empty)
                                                        , new XAttribute("EndDate", x.EndDate ?? string.Empty)
                                                        , new XAttribute("RateMin", x.RateMin ?? null)
                                                        , new XAttribute("RateMax", x.RateMax ?? string.Empty)));

                if (toAdd.Count() > 0)
                {
                    var add = new XElement("Jobs", toAdd);
                    
                    return await JobDal.Instance.SynchJobsXML(add.ToString());
                }
                else
                {
                    jsonResult = new { Result = "1", Message = "Jobs are already synchronized" };
                }
            }
            catch (Exception ex)
            {
                ErrorLog error = new ErrorLog()
                {
                    ErrorCode = -1,
                    Controller = "Job",
                    Method = "Api/Job/SynchJobsXML",
                    InputValue = "",
                    InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };

                await Common.LogErrorInDBAsync(error);
                jsonResult = new
                {
                    Result = "0",
                    Message = "Live Jobs not loaded, Please try again."
                };
            }

            return JsonConvert.SerializeObject(jsonResult);
        }

        private static List<WorkFlow.Models.Job> GetDBJobs()
        {
            List<WorkFlow.Models.Job> jobs = new List<WorkFlow.Models.Job>();

            DAL objDal = new DAL();
            object[][] objParam = new object[][]
            {
                new object[] { "@status", -1 }
            };

            DataSet dsResult = objDal.ExecuteDS("spGetJobList", objParam);

            if (dsResult != null && dsResult.Tables.Count > 0)
            {
                foreach (DataRow row in dsResult.Tables[0].Rows)
                {
                    WorkFlow.Models.Job j1 = new WorkFlow.Models.Job()
                    {
                        JobCode = row["referenceid"].ToString(),
                        Title = row["title"].ToString(),
                        Location = row["location"].ToString(),
                        Description = row["description"].ToString(),
                        Link = "",                        
                        PubDate = row["publisheddate"].ToString(),
                        StartDate = row["startdate"].ToString(),
                        EndDate = row["enddate"].ToString(),
                        IsActive = Convert.ToInt32(row["isactive"])
                    };
                    jobs.Add(j1);
                }
            }

            return jobs;
        }

        private static List<WorkFlow.Models.Job> GetRSSFeedJobs()
        {
            XmlDocument rssXmlDoc = new XmlDocument();

            // Load the RSS file from the RSS URL
            rssXmlDoc.Load(ConfigurationManager.AppSettings["RSSFeedPath"].ToString());

            // Parse the Items in the RSS file
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("rss/channel/item");

            StringBuilder rssContent = new StringBuilder();

            List<WorkFlow.Models.Job> jobs = new List<WorkFlow.Models.Job>();

            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                string title = rssSubNode != null ? rssSubNode.InnerText : "";

                rssSubNode = rssNode.SelectSingleNode("link");
                string link = rssSubNode != null ? rssSubNode.InnerText : "";

                rssSubNode = rssNode.SelectSingleNode("description");
                string description = rssSubNode != null ? rssSubNode.InnerText : "";

                rssSubNode = rssNode.SelectSingleNode("pubDate");
                //string pubDate = rssSubNode != null ? rssSubNode.InnerText : "";
                string pubDate = rssSubNode != null ? getDateTime(rssSubNode.InnerText) : "";

                rssContent.Append("<a href='" + link + "'>" + title + "</a><br>" + description);

                string loc = string.Empty, jobCode = string.Empty;

                if (!string.IsNullOrWhiteSpace(title))
                {
                    loc = title.Substring(title.IndexOf(") -") + 3);

                    if (!string.IsNullOrWhiteSpace(loc))
                    {
                        title = title.Replace(loc, "").Trim();
                        loc = loc.Trim();
                    }

                    string data = title.Substring(title.LastIndexOf("(") + 1);

                    title = title.Replace(data, "").Trim();
                    title = title.Substring(0, title.Length - 1).Trim();

                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        jobCode = data.Substring(0, data.LastIndexOf(")"));
                    }
                }

                WorkFlow.Models.Job job = new WorkFlow.Models.Job()
                {
                    Title = title,
                    JobCode = jobCode,
                    Location = loc,
                    Link = link,
                    Description = description,
                    PubDate = pubDate
                };

                jobs.Add(job);
            }

            return jobs;
        }
        #endregion

        private static List<WorkFlow.Models.Job> GetXMLFeedJobs()
        {
            List<WorkFlow.Models.Job> jobs = new List<WorkFlow.Models.Job>();
            XmlDocument rssXmlDoc = new XmlDocument();            
            string path = ConfigurationManager.AppSettings["XMLFeedPath"].ToString();

            /* Start - Below code is required for JobsDiva site. */
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            /* End - Above code is required for JobsDiva site. */

            rssXmlDoc.Load(path);

            // Parse the Items in the site
            XmlNodeList rssNodes = rssXmlDoc.SelectNodes("outertag/jobs/job");

            StringBuilder rssContent = new StringBuilder();

            // Iterate through the items in the RSS file
            foreach (XmlNode rssNode in rssNodes)
            {
                string title, jobCode, location, description, pubDate, startDate, endDate, rateMin, rateMax;
                XmlNode rssSubNode = rssNode.SelectSingleNode("title");
                title = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("jobdiva_no");
                jobCode = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("location");
                location = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("jobdescription");
                description = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("issuedate");
                pubDate = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("startdate");
                startDate = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("endddate");
                endDate = rssSubNode != null ? rssSubNode.InnerText : string.Empty;

                rssSubNode = rssNode.SelectSingleNode("ratemin");
                rateMin = rssSubNode != null ? rssSubNode.InnerText : null;

                rssSubNode = rssNode.SelectSingleNode("ratemax");
                rateMax = rssSubNode != null ? rssSubNode.InnerText : null;

                WorkFlow.Models.Job job = new WorkFlow.Models.Job()
                {
                    Title = title,
                    JobCode = jobCode,
                    Location = location,
                    Description = description,
                    PubDate = pubDate,
                    StartDate = startDate,
                    EndDate = endDate,
                    RateMin = rateMin,
                    RateMax = rateMax
                };

                jobs.Add(job);
            }
            
            return jobs;
        }

        public static string SerializeAnObject(Object item)
        {
            if (item == null)
                return null;

            var stringBuilder = new StringBuilder();
            var itemType = item.GetType();

            new XmlSerializer(itemType).Serialize(new StringWriter(stringBuilder), item);

            return stringBuilder.ToString();
        }
        
        private static string getDateTime(string date)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(date))
                    return string.Empty;

                string am = " AM ", pm = " PM ", day = string.Empty;
                List<string> days = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                if (date.Contains(am))
                    date = date.Substring(0, date.IndexOf(am)+3);
                else if (date.Contains(pm))
                    date = date.Substring(0, date.IndexOf(pm)+3);
                
                if(date.Contains(","))
                    day = date.Substring(0, date.IndexOf(","));

                if (days.Contains(day))
                    date = date.Substring(day.Length + 1);
                return date.Trim(); // OR  Convert.ToDateTime(date);
            }
            catch (Exception ex)
            {
                /* Ensure both the date format are same. */
                return DateTime.Now.ToString();  //Or DateTime.Now;
            }
        }


        /* TODO - Remove.  Moved to another Dal/Controller. */

    }
}
