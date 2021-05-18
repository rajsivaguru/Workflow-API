using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StaffingService.DataAccess
{
    public class RecruiterDal
    {
        private static ISqlDbConnection _dbConnection;
        private static RecruiterDal instance = null;
        private static readonly object padlock = new object();

        public RecruiterDal()
        {
        }

        public static RecruiterDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new RecruiterDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }


        internal async Task<ResponseModel> GetRecruiterJobList(int userId)
        {
            ResponseModel response = new ResponseModel();
            List<RecruitersJobs> result = new List<RecruitersJobs>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@UserId", userId, DbType.Int32);

                var data = await conn.QueryAsync<RecruitersJobs>(Constants.StoredProcedure.GETMYJOBLIST, param, null, null, CommandType.StoredProcedure);
                result = (List<RecruitersJobs>)data;

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }
        
        internal async Task<ResponseModel> StartRecruiterJob(int jobassignmentId, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@JobAssignmentId", jobassignmentId, DbType.Int32);
                param.Add("@UserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEMYJOB, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = Constants.ResponseResult.SUCCESS;
                response.RequestType = Constants.RequestType.POST;
                response.Output = result;
                response.SuccessMessage = result == 0 ? string.Empty : "Job started.";
                response.ErrorMessage = result == 0 ? "Error occurred while starting the job.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> StopRecruiterJob(RecruitersJobs source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@JobAssignmentStatusId", source.jobassignmentstatusid, DbType.Int32);
                param.Add("@JobAssignmentId", source.jobassignmentid, DbType.Int32);
                param.Add("@Submission", source.submission, DbType.Int32);
                param.Add("@NotesAdded", source.notesadded, DbType.Int32);
                param.Add("@QualificationAdded", source.qualificationadded, DbType.Int32);
                param.Add("@Comment", source.comment, DbType.String);
                param.Add("@UserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEMYJOB, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Job stopped.";
                response.ErrorMessage = result == 0 ? "Error occurred while stopping the job.  Please try again." : string.Empty;
            }

            return response;
        }
    }
}