using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffingService.DataAccess
{
    public class AttendanceDal
    {
        private static ISqlDbConnection _dbConnection;
        private static AttendanceDal instance = null;
        private static readonly object padlock = new object();

        public AttendanceDal()
        {
        }

        public static AttendanceDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new AttendanceDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }


        internal async Task<ResponseModel> GetMyPunchDetails(int loginId)
        {
            ResponseModel response = new ResponseModel();
            List<PunchInOut> result = new List<PunchInOut>();

            if(loginId == 0)
            {
                response = Common.GetResponse(result.Count());
                response.Output = result;
                return response;
            }

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@LoginId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<PunchInOut>(Constants.StoredProcedure.GETMYPUNCHDETAILS, param, null, null, CommandType.StoredProcedure);
                result = (List<PunchInOut>)data;

                response = Common.GetResponse(data.Count());
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveMyPunchDetails(PunchInOut source, int loginUserId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                
                param.Add("@LoginId", loginUserId, DbType.Int32);
                param.Add("@PunchId", source.punchid, DbType.Int32);
                param.Add("@PunchDay", source.punchday, DbType.Date);
                param.Add("@InTime", source.intime, DbType.DateTimeOffset);

                if(source.outtime.HasValue)
                {
                    param.Add("@OutTime", source.outtime, DbType.DateTimeOffset);
                }
                                
                param.Add("@Notes", source.notes.Trim(), DbType.String);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SAVEMYPUNCHDETAILS, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Punch In/Out saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }

    }
}