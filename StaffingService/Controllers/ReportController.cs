using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace StaffingService.Controllers
{
    [CustomExceptionFilter]
    public class ReportController : ApiController
    {
        public ReportController()
        {

        }

        [HttpGet]
        public async Task<string> GetPeriods()
        {
            return await ReportDal.Instance.GetPeriods();
        }

        [HttpGet]
        public async Task<string> GetUsersForReport(int statusId, bool isAllUser, int loginId)
        {
            return await ReportDal.Instance.GetUsersForReport(statusId, isAllUser);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetJobReport(string source)
        {
            JobReportParam param = JsonConvert.DeserializeObject<JobReportParam>(source);
            ResponseModel result = await ReportDal.Instance.GetJobReport(param);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetJobReportFile(string sourceParam)
        {
            HttpResponse response = HttpContext.Current.Response;
            List<JobReport> reportData = new List<JobReport>();
            string clientids = string.Empty;

            JobReportParam source = JsonConvert.DeserializeObject<JobReportParam>(sourceParam);
            var reportResponse = await ReportDal.Instance.GetJobReport(source);

            if (reportResponse.Output != null)
                reportData = (List<JobReport>)reportResponse.Output;

            /* Follow the below URL, so report will work.             
             https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
             */
            try
            {
                byte[] bytes;
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;

                LocalReport lr = new LocalReport();
                lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\JobReport.rdlc");

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("JobCode", source.referenceid));
                parameters.Add(new ReportParameter("Title", source.title));
                parameters.Add(new ReportParameter("Location", source.location));
                parameters.Add(new ReportParameter("PublishedDate", source.publisheddate));
                parameters.Add(new ReportParameter("IsActive", source.status.ToString()));
                parameters.Add(new ReportParameter("FromDate", source.fromdate));
                parameters.Add(new ReportParameter("ToDate", source.todate));
                parameters.Add(new ReportParameter("LastDays", source.lastdays.ToString()));
                lr.SetParameters(parameters);

                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("DsJobReport", reportData));
                lr.Refresh();

                /* < PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight > */
                string deviceInfo = @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>11.5in</PageWidth>
                    <PageHeight>8.5in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.15in</MarginLeft>
                    <MarginRight>0.15in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

                if (source.reporttype == "excel")
                {
                    bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }
                else
                {
                    bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }

                generateFile(response, bytes, "JobReport_", source.reporttype);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetUserReport(string source, int loginid)
        {
            UserReportParam param = JsonConvert.DeserializeObject<UserReportParam>(source);
            ResponseModel result = await ReportDal.Instance.GetUserReport(param, loginid);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetClientReport(string source)
        {
            ClientReportParam param = JsonConvert.DeserializeObject<ClientReportParam>(source);
            ResponseModel result = await ReportDal.Instance.GetClientReport(param);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetProfileSearchReport(string source)
        {
            ProfileSearchReportParam param = JsonConvert.DeserializeObject<ProfileSearchReportParam>(source);
            ResponseModel result = await ReportDal.Instance.GetProfileSearchReport(param);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetPunchReport(string source)
        {
            PunchReportParam param = JsonConvert.DeserializeObject<PunchReportParam>(source);
            ResponseModel result = await ReportDal.Instance.GetPunchReport(param);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetUserReportFile(string sourceParam)
        {
            HttpResponse response = HttpContext.Current.Response;
            List<UserReport> reportData = new List<UserReport>();
            string userIds = string.Empty;

            UserReportParam source = JsonConvert.DeserializeObject<UserReportParam>(sourceParam);
            var reportResponse = await ReportDal.Instance.GetUserReport(source, 0);

            if (reportResponse.Output != null)
                reportData = (List<UserReport>)reportResponse.Output;

            /* Follow the below URL, so report will work.             
             https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
             */
            try
            {
                byte[] bytes;
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;

                LocalReport lr = new LocalReport();
                lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\UserReport.rdlc");

                if (source.userids != null && source.userids.Count > 0)
                    userIds = string.Join(",", source.userids);

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("UserIds", userIds));
                parameters.Add(new ReportParameter("JobCode", source.jobcode));
                parameters.Add(new ReportParameter("Title", source.title));
                parameters.Add(new ReportParameter("Location", source.location));
                parameters.Add(new ReportParameter("PublishedDate", source.publisheddate));
                parameters.Add(new ReportParameter("AssignedDate", source.assingeddate));
                parameters.Add(new ReportParameter("FromDate", source.fromdate));
                parameters.Add(new ReportParameter("ToDate", source.todate));
                parameters.Add(new ReportParameter("LastDays", source.lastdays.ToString()));
                lr.SetParameters(parameters);

                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("DsUserReport", reportData));
                lr.Refresh();

                //< PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight >
                string deviceInfo = @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>11.5in</PageWidth>
                    <PageHeight>8.5in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.15in</MarginLeft>
                    <MarginRight>0.15in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

                if (source.reporttype == "excel")
                {
                    bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }
                else
                {
                    bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }

                generateFile(response, bytes, "UserReport_", source.reporttype);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetClientReportFile(string sourceParam)
        {
            HttpResponse response = HttpContext.Current.Response;
            List<ClientReport> reportData = new List<ClientReport>();
            string clientids = string.Empty;

            ClientReportParam source = JsonConvert.DeserializeObject<ClientReportParam>(sourceParam);
            var reportResponse = await ReportDal.Instance.GetClientReport(source);

            if (reportResponse.Output != null)
                reportData = (List<ClientReport>)reportResponse.Output;

            /* Follow the below URL, so report will work.             
             https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
             */
            try
            {
                byte[] bytes;
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;

                LocalReport lr = new LocalReport();
                lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\ClientReport.rdlc");

                if (source.clientids != null && source.clientids.Count > 0)
                    clientids = string.Join(",", source.clientids);

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("ClientIds", clientids));
                parameters.Add(new ReportParameter("JobCode", source.jobcode));
                parameters.Add(new ReportParameter("Title", source.title));
                parameters.Add(new ReportParameter("PublishedDate", source.publisheddate));
                parameters.Add(new ReportParameter("LastDays", source.lastdays.ToString()));
                lr.SetParameters(parameters);

                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("DsClientReport", reportData));
                lr.Refresh();

                /* < PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight > */
                string deviceInfo = @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>11.5in</PageWidth>
                    <PageHeight>8.5in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.15in</MarginLeft>
                    <MarginRight>0.15in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

                if (source.reporttype == "excel")
                {
                    bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }
                else
                {
                    bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }

                generateFile(response, bytes, "ClientReport_", source.reporttype);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetProfileSearchReportFile(string sourceParam)
        {
            HttpResponse response = HttpContext.Current.Response;
            List<ProfileSearchReport> reportData = new List<ProfileSearchReport>();
            string userids = string.Empty;

            ProfileSearchReportParam source = JsonConvert.DeserializeObject<ProfileSearchReportParam>(sourceParam);
            var reportResponse = await ReportDal.Instance.GetProfileSearchReport(source);

            if (reportResponse.Output != null)
                reportData = (List<ProfileSearchReport>)reportResponse.Output;

            /* Follow the below URL, so report will work.             
             https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
             */
            try
            {
                byte[] bytes;
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;

                LocalReport lr = new LocalReport();
                lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\ProfileSearchReport.rdlc");

                if (source.userids != null && source.userids.Count > 0)
                    userids = string.Join(",", source.userids);

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("UserIds", userids));
                parameters.Add(new ReportParameter("JobCode", source.jobcode));
                parameters.Add(new ReportParameter("Title", source.title));
                parameters.Add(new ReportParameter("Location", source.location));
                parameters.Add(new ReportParameter("SearchedDate", source.searcheddate));
                parameters.Add(new ReportParameter("LastDays", source.lastdays.ToString()));
                lr.SetParameters(parameters);

                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("DsProfileSearchReport", reportData));
                lr.Refresh();

                /* < PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight > */
                string deviceInfo = @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>11.5in</PageWidth>
                    <PageHeight>8.5in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.15in</MarginLeft>
                    <MarginRight>0.15in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

                if (source.reporttype == "excel")
                {
                    bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }
                else
                {
                    bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }

                generateFile(response, bytes, "ProfileSearchReport_", source.reporttype);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        /* Need to develop */
        [HttpGet]
        public async Task<HttpResponseMessage> GetPunchReportFile(string sourceParam)
        {
            HttpResponse response = HttpContext.Current.Response;
            List<ProfileSearchReport> reportData = new List<ProfileSearchReport>();
            string userids = string.Empty;

            PunchReportParam source = JsonConvert.DeserializeObject<PunchReportParam>(sourceParam);
            var reportResponse = await ReportDal.Instance.GetPunchReport(source);

            if (reportResponse.Output != null)
                reportData = (List<PunchReport>)reportResponse.Output;

            /* Follow the below URL, so report will work.             
             https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
             */
            try
            {
                byte[] bytes;
                Warning[] warnings;
                string[] streamids;
                string mimeType, encoding, extension;

                LocalReport lr = new LocalReport();
                lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\PunchReport.rdlc");

                if (source.userids != null && source.userids.Count > 0)
                    userids = string.Join(",", source.userids);

                List<ReportParameter> parameters = new List<ReportParameter>();
                parameters.Add(new ReportParameter("UserIds", userids));
                parameters.Add(new ReportParameter("ShowOnlyMissingTime", source.showonlymissingtime.ToString()));
                parameters.Add(new ReportParameter("IncludeWeekEnds", source.includeweekends.ToString()));
                parameters.Add(new ReportParameter("FromDate", source.fromdate));
                parameters.Add(new ReportParameter("ToDate", source.todate));
                lr.SetParameters(parameters);

                lr.DataSources.Clear();
                lr.DataSources.Add(new ReportDataSource("DsPunchReport", reportData));
                lr.Refresh();

                /* < PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight > */
                string deviceInfo = @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>11.5in</PageWidth>
                    <PageHeight>8.5in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.15in</MarginLeft>
                    <MarginRight>0.15in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

                if (source.reporttype == "excel")
                {
                    bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }
                else
                {
                    bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
                }

                generateFile(response, bytes, "PunchReport_", source.reporttype);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        private void generateFile(HttpResponse response, byte[] bytes, string filename, string reporttype)
        {
            response.ClearContent();
            response.Buffer = true;

            if (reporttype == "excel")
            {
                response.AddHeader("content-disposition", "attachment; filename=" + filename + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
                response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            }
            else
            {
                response.AddHeader("content-disposition", "attachment; filename=" + filename + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                response.ContentType = "application/pdf";
            }

            response.Charset = "charset=utf-8";
            response.BinaryWrite(bytes);
            response.End();
        }


        #region For Testing ONLY

        //[HttpGet]
        //public async Task<string> GetJobReportFileTest(string sourceParam)
        //{
        //    string line = "1";
        //    HttpResponse response = HttpContext.Current.Response;
        //    List<JobReport> reportData = new List<JobReport>();
        //    string clientids = string.Empty;
        //    line = "2";
        //    JobReportParam source = JsonConvert.DeserializeObject<JobReportParam>(sourceParam);
        //    var reportResponse = await ReportDal.Instance.GetJobReport(source);
        //    line = "3";
        //    if (reportResponse.Output != null)
        //        reportData = (List<JobReport>)reportResponse.Output;
        //    line = "4";
        //    /* Follow the below URL, so report will work.             
        //     https://stackoverflow.com/questions/38902037/ssrs-report-definition-is-newer-than-server?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
        //     */
        //    try
        //    {
        //        line = "5";
        //        byte[] bytes;
        //        Warning[] warnings;
        //        string[] streamids;
        //        string mimeType, encoding, extension;
        //        line = "6";
        //        LocalReport lr = new LocalReport();
        //        lr.ReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports\\JobReport.rdlc");
        //        //lr.ReportPath = Path.Combine("http://apps.techdigitalcorp.com/WorkflowApi-Dev/", "Reports/JobReport.rdlc");
        //        line = "7";
        //        List<ReportParameter> parameters = new List<ReportParameter>();
        //        parameters.Add(new ReportParameter("JobCode", source.referenceid));
        //        parameters.Add(new ReportParameter("Title", source.title));
        //        parameters.Add(new ReportParameter("Location", source.location));
        //        parameters.Add(new ReportParameter("PublishedDate", source.publisheddate));
        //        parameters.Add(new ReportParameter("IsActive", source.status.ToString()));
        //        parameters.Add(new ReportParameter("FromDate", source.fromdate));
        //        parameters.Add(new ReportParameter("ToDate", source.todate));
        //        parameters.Add(new ReportParameter("LastDays", source.lastdays.ToString()));
        //        line = "7.1";
        //        lr.SetParameters(parameters);
        //        line = "8";
        //        lr.DataSources.Clear();
        //        lr.DataSources.Add(new ReportDataSource("DsJobReport", reportData));
        //        lr.Refresh();
        //        line = "9";
        //        /* < PageWidth > 8.5in</ PageWidth > < PageHeight > 11in</ PageHeight > */
        //        string deviceInfo = @"<DeviceInfo>
        //            <OutputFormat>EMF</OutputFormat>
        //            <PageWidth>11.5in</PageWidth>
        //            <PageHeight>8.5in</PageHeight>
        //            <MarginTop>0.25in</MarginTop>
        //            <MarginLeft>0.15in</MarginLeft>
        //            <MarginRight>0.15in</MarginRight>
        //            <MarginBottom>0.25in</MarginBottom>
        //        </DeviceInfo>";
        //        line = "10";
        //        if (source.reporttype == "excel")
        //        {
        //            bytes = lr.Render("EXCELOPENXML", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
        //        }
        //        else
        //        {
        //            line = "11";
        //            bytes = lr.Render("pdf", deviceInfo, out mimeType, out encoding, out extension, out streamids, out warnings);
        //            line = "12";
        //        }
        //        line = "13";
        //        generateFile(response, bytes, "JobReport_", source.reporttype);
        //        line = "14";
        //        return "success";
        //    }
        //    catch (Exception ex)
        //    {
        //        string innerException = string.Empty;
        //        if (ex.InnerException != null)
        //            innerException = ex.InnerException.ToString();
        //        if (ex.InnerException.InnerException != null)
        //            innerException = innerException + " ~ another inner ~: " + ex.InnerException.InnerException.ToString();

        //        return line + " ~ " + ex.Message + " ` " + innerException;
        //    }
        //}
        #endregion

    }
}