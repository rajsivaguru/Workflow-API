using Dapper;
using Newtonsoft.Json;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace StaffingService.DataAccess
{
    internal class ReportDal
    {
        private static ISqlDbConnection _dbConnection;

        private static ReportDal instance = null;
        private static readonly object padlock = new object();
        
        private ReportDal()
        {
            
        }
        
        public static ReportDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ReportDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }


        public async Task<string> GetPeriods()
        {
            using (IDbConnection conn = _dbConnection.Connection)
            {
                dynamic data = await conn.QueryAsync<Period>(Constants.StoredProcedure.GETPERIOD, null, null, null, CommandType.StoredProcedure);
                return JsonConvert.SerializeObject((List<Period>)data).ToString();
            }

            return string.Empty;
        }

        internal async Task<string> GetUsersForReport(int statusId, bool getAllUsers)
        {
            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@status", statusId, DbType.Int32);
                if (!getAllUsers)
                    param.Add("@isAll", getAllUsers, DbType.Boolean);

                dynamic data = await conn.QueryAsync<UserAssignmentDto>(Constants.StoredProcedure.GETUSERSFORREPORT, param, null, null, CommandType.StoredProcedure);

                List<UserAssignment> users = new List<UserAssignment>();
                ((List<UserAssignmentDto>)data).ToList().ForEach((x) =>
                {
                    users.Add(new UserAssignment()
                    {
                        userid = x.userid,
                        email = x.email,
                        name = Common.GetFullName(x.fname, x.minitial, x.lname, x.email),
                        roleid = x.roleid,
                        rolename = x.rolename,
                        imgurl = x.imgurl
                    });
                });

                return JsonConvert.SerializeObject(users).ToString();
            }
        }
        
        internal async Task<ResponseModel> GetJobReport(JobReportParam parameters)
        {
            ResponseModel response = new ResponseModel();
            List<JobReport> result = new List<JobReport>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                string clientids = string.Empty;

                if (!string.IsNullOrWhiteSpace(parameters.referenceid) || !string.IsNullOrWhiteSpace(parameters.jobcode))
                    param.Add("@JobCode", string.IsNullOrWhiteSpace(parameters.referenceid) ? parameters.jobcode : parameters.referenceid, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.title))
                    param.Add("@Title", parameters.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.location))
                    param.Add("@Location", parameters.location, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.publisheddate))
                    param.Add("@PublishedDate", parameters.publisheddate, DbType.String);
                if (parameters.status >= 0)
                    param.Add("@IsActive", parameters.status, DbType.Int16);
                if (!string.IsNullOrWhiteSpace(parameters.fromdate))
                    param.Add("@FromDate", parameters.fromdate, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.todate))
                    param.Add("@ToDate", parameters.todate, DbType.String);
                if (parameters.lastdays != -1)
                    param.Add("@LastDays", parameters.lastdays, DbType.Int32);

                dynamic data = await conn.QueryAsync<JobReport>(Constants.StoredProcedure.GETJOBREPORT, param, null, null, CommandType.StoredProcedure);
                result = (List<JobReport>)data;

                response.ResultStatus = result.Count > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA;
                response.Output = result;
                response.OutputCount = result.Count;
            }

            return response;
        }

        internal async Task<ResponseModel> GetUserReport(UserReportParam parameters, int loginId)
        {
            ResponseModel response = new ResponseModel();
            List<UserReport> result = new List<UserReport>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                /*
                    var valueIds = new DataTable();

                    if (parameters.userids == null)
                        parameters.userids = new List<int>() { 0 };

                    valueIds.Columns.Add("ID", typeof(string));
                    parameters.userids.ForEach(x =>
                    {
                        var dr = valueIds.NewRow();
                        dr[0] = x;
                        valueIds.Rows.Add(dr);
                    });

                    valueIds.AcceptChanges();

                    param.Add("@UserIds", valueIds.AsTableValuedParameter("TABLE_ID_INT"));
                */

                string userIds = string.Empty;
                if (parameters.userids != null && parameters.userids.Count > 0)
                    userIds = string.Join(",", parameters.userids);

                if (!string.IsNullOrWhiteSpace(userIds))
                    param.Add("@UserIds", userIds, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.jobcode))
                    param.Add("@JobCode", parameters.jobcode, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.title))
                    param.Add("@Title", parameters.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.location))
                    param.Add("@Location", parameters.location, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.publisheddate))
                    param.Add("@PublishedDate", parameters.publisheddate, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.assingeddate))
                    param.Add("@AssignedDate", parameters.assingeddate, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.fromdate))
                    param.Add("@FromDate", parameters.fromdate, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.todate))
                    param.Add("@ToDate", parameters.todate, DbType.String);
                if (parameters.lastdays != -1)
                    param.Add("@LastDays", parameters.lastdays, DbType.Int32);

                dynamic data = await conn.QueryAsync<UserReport>(Constants.StoredProcedure.GETUSERREPORT, param, null, null, CommandType.StoredProcedure);
                result = (List<UserReport>)data;

                response.ResultStatus = result.Count > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA;
                response.Output = result;
                response.OutputCount = result.Count;
            }

            return response;
        }

        internal async Task<ResponseModel> GetClientReport(ClientReportParam parameters)
        {
            ResponseModel response = new ResponseModel();
            List<ClientReport> result = new List<ClientReport>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                string clientids = string.Empty;
                if (parameters.clientids != null && parameters.clientids.Count > 0)
                    clientids = string.Join(",", parameters.clientids);

                if (!string.IsNullOrWhiteSpace(clientids))
                    param.Add("@ClientIds", clientids, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.jobcode))
                    param.Add("@JobCode", parameters.jobcode, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.title))
                    param.Add("@Title", parameters.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.publisheddate))
                    param.Add("@PublishedDate", parameters.publisheddate, DbType.String);
                if (parameters.lastdays != -1)
                    param.Add("@LastDays", parameters.lastdays, DbType.Int32);

                dynamic data = await conn.QueryAsync<ClientReport>(Constants.StoredProcedure.GETCLIENTREPORT, param, null, null, CommandType.StoredProcedure);
                result = (List<ClientReport>)data;

                response.ResultStatus = result.Count > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA;
                response.Output = result;
                response.OutputCount = result.Count;
            }

            return response;
        }

        internal async Task<ResponseModel> GetProfileSearchReport(ProfileSearchReportParam parameters)
        {
            ResponseModel response = new ResponseModel();
            List<ProfileSearchReport> result = new List<ProfileSearchReport>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                string userIds = string.Empty;
                if (parameters.userids != null && parameters.userids.Count > 0)
                    userIds = string.Join(",", parameters.userids);

                if (!string.IsNullOrWhiteSpace(userIds))
                    param.Add("@UserIds", userIds, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.jobcode))
                    param.Add("@JobCode", parameters.jobcode, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.title))
                    param.Add("@Title", parameters.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.location))
                    param.Add("@Location", parameters.location, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.searcheddate))
                    param.Add("@SearchedDate", parameters.searcheddate, DbType.String);
                if (parameters.lastdays != -1)
                    param.Add("@LastDays", parameters.lastdays, DbType.Int32);

                dynamic data = await conn.QueryAsync<ProfileSearchReport>(Constants.StoredProcedure.GETPROFILESEACHRREPORT, param, null, null, CommandType.StoredProcedure);
                result = (List<ProfileSearchReport>)data;

                response.ResultStatus = result.Count > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA;
                response.Output = result;
                response.OutputCount = result.Count;
            }

            return response;
        }

        internal async Task<ResponseModel> GetPunchReport(ProfileSearchReportParam parameters)
        {
            ResponseModel response = new ResponseModel();
            List<ProfileSearchReport> result = new List<ProfileSearchReport>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                string userIds = string.Empty;
                if (parameters.userids != null && parameters.userids.Count > 0)
                    userIds = string.Join(",", parameters.userids);

                if (!string.IsNullOrWhiteSpace(userIds))
                    param.Add("@UserIds", userIds, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.jobcode))
                    param.Add("@JobCode", parameters.jobcode, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.title))
                    param.Add("@Title", parameters.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.location))
                    param.Add("@Location", parameters.location, DbType.String);
                if (!string.IsNullOrWhiteSpace(parameters.searcheddate))
                    param.Add("@SearchedDate", parameters.searcheddate, DbType.String);
                if (parameters.lastdays != -1)
                    param.Add("@LastDays", parameters.lastdays, DbType.Int32);

                dynamic data = await conn.QueryAsync<ProfileSearchReport>(Constants.StoredProcedure.GETPROFILESEACHRREPORT, param, null, null, CommandType.StoredProcedure);
                result = (List<ProfileSearchReport>)data;

                response.ResultStatus = result.Count > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA;
                response.Output = result;
                response.OutputCount = result.Count;
            }

            return response;
        }

    }
}