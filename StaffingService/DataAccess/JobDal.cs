using Dapper;
using Newtonsoft.Json;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffingService.DataAccess
{
    internal class JobDal
    {
        private static ISqlDbConnection _dbConnection;
        private static JobDal instance = null;
        private static readonly object padlock = new object();

        public JobDal()
        {
        }

        public static JobDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new JobDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }

        internal async Task<string> SynchJobsXML(string source)
        {
            var jsonResult = new
            {
                Result = "0",
                Message = "Live Jobs not loaded, Please try again."
            };

            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@XMLJobs", source, DbType.String);
                    
                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SYNCJOB, param, null, null, CommandType.StoredProcedure);
                    int res = 0;

                    if (data.ToList().Count > 0)
                        res = data.ToList()[0];

                    jsonResult = new
                    {
                        Result = res.ToString(),
                        Message = res == 0 ? "Error occurred while sychronizing job.  Please try again." : "Live jobs synched."
                    };

                    return JsonConvert.SerializeObject(jsonResult).ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return JsonConvert.SerializeObject(jsonResult);
        }

        internal async Task<ResponseModel> GetJobDetails(int id, string reference)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                param.Add("@Reference", reference, DbType.String);

                var data = await conn.QueryAsync<JobDetail>(Constants.StoredProcedure.GETJOBDETAILS, param, null, null, CommandType.StoredProcedure);

                response.ResultStatus = 1;
                response.ErrorMessage = string.Empty;
                response.OutputCount = data.Count();
                response.Output = data;
            }

            return response;
        }

        internal async Task<ResponseModel> GetJobList(int loginid)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", loginid, DbType.Int32);

                var data = await conn.QueryAsync<Job>(Constants.StoredProcedure.GETJOBS, param, null, null, CommandType.StoredProcedure);

                response.ResultStatus = 1;
                response.ErrorMessage = string.Empty;
                response.OutputCount = data.Count();
                response.Output = data;
            }

            return response;
        }

        internal async Task<ResponseModel> GetJobListForDD(int loginid)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<JobFormatted>(Constants.StoredProcedure.GETJOBS_DD, null, null, null, CommandType.StoredProcedure);

                response.ResultStatus = 1;
                response.ErrorMessage = string.Empty;
                response.OutputCount = data.Count();
                response.Output = data;
            }

            return response;
        }

        internal async Task<ResponseModel> GetJobPriorityList(int loginid, int isAll = 0)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@LoginId", loginid, DbType.Int32);
                param.Add("@IsAll", isAll, DbType.Boolean);

                var data = await conn.QueryAsync<PriorityJob>(Constants.StoredProcedure.GETPRIORITYJOBS, param, null, null, CommandType.StoredProcedure);

                response.ResultStatus = 1;
                response.ErrorMessage = string.Empty;
                response.OutputCount = data.Count();
                response.Output = data;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveJobAssignment(JobAssignment assignment)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@jobid", assignment.jobid, DbType.Int32);
                param.Add("@priorityid", assignment.priorityid, DbType.Int32);
                param.Add("@loginid", assignment.loginid, DbType.Int32);

                if (assignment.userids.Count > 0)
                    param.Add("@userid", string.Join(",", assignment.userids), DbType.String);
                if (!string.IsNullOrWhiteSpace(assignment.clientname))
                    param.Add("@clientname", assignment.clientname.Trim(), DbType.String);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.ASSIGNJOB, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Job assigned successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while assigning job.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveJobsAssignment(List<JobAssignment> assignments)
        {
            ResponseModel response = new ResponseModel();
            
            int successCount = 0;

            for(int i = 0; i < assignments.Count; i++)
            {
                int result = await saveJobAssignment(assignments[i]);
                if (result > 0)
                    successCount++;
            }

            response.ResultStatus = successCount != assignments.Count ? 0 : 1;
            response.ErrorMessage = successCount != assignments.Count ? $"Error occurred while assigning {assignments.Count - successCount} job.  Please try again." : string.Empty;
            response.SuccessMessage = "Job(s) assigned successfully.";

            return response;
        }

        internal async Task<ResponseModel> SavePriorityJob(string jobIds, int loginId)
        {
            ResponseModel response = new ResponseModel();
            
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@JobIds", jobIds, DbType.String);
                    param.Add("@LoginId", loginId, DbType.Int32);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.ASSIGNPRIORITYJOB, param, null, null, CommandType.StoredProcedure);
                    int result = 0;

                    if (data.ToList().Count > 0)
                        result = data.ToList()[0];

                    response.ResultStatus = result;
                    response.RequestType = Constants.RequestType.POST;
                    response.SuccessMessage = result == 0 ? string.Empty : "Priority Job saved successfully.";
                    response.ErrorMessage = result == 0 ? "Error occurred while prioritizing job.  Please try again." : string.Empty;
                }
            }
            catch (Exception ex)
            {

            }
            
            return response;
        }

        internal async Task<ResponseModel> SaveInterestedJob(int jobId, int loginId)
        {
            ResponseModel response = new ResponseModel();

            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@JobId", jobId, DbType.Int32);
                    param.Add("@UserId", loginId, DbType.Int32);
                    param.Add("@NotificationName", "Admin_DL_TL", DbType.String);
                    param.Add("@NotificationType", "Job Interest", DbType.String);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SaveInterestedJob, param, null, null, CommandType.StoredProcedure);
                    int result = 0;

                    if (data.ToList().Count > 0)
                        result = data.ToList()[0];

                    response.Output = result;
                    response.ResultStatus = 1;
                    response.RequestType = Constants.RequestType.POST;
                    response.SuccessMessage = result == 0 ? string.Empty : "Your job interest has been notified.";
                    response.ErrorMessage = result == 0 ? "Error occurred while saving your job interest.  Please try again." : string.Empty;
                }
            }
            catch (Exception ex)
            {

            }

            return response;
        }

        
        private async Task<int> saveJobAssignment(JobAssignment assignment)
        {
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@jobid", assignment.jobid, DbType.Int32);
                    param.Add("@priorityid", assignment.priorityid, DbType.Int32);
                    param.Add("@clientname", assignment.clientname.Trim(), DbType.String);
                    param.Add("@loginid", assignment.loginid, DbType.Int32);

                    if (assignment.userids.Count > 0)
                        param.Add("@userid", string.Join(",", assignment.userids), DbType.String);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.ASSIGNJOB, param, null, null, CommandType.StoredProcedure);
                    int result = 0;

                    if (data.ToList().Count > 0)
                        result = data.ToList()[0];

                    return result;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        
    }
}
