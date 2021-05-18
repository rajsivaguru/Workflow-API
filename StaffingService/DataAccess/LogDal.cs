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
    public class LogDal
    {
        private static ISqlDbConnection _dbConnection;
        private static LogDal instance = null;
        private static readonly object padlock = new object();

        private LogDal()
        {

        }

        public static LogDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new LogDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }
        
        public void LogError(ErrorLog error)
        {
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@ErrorCode", error.ErrorCode, DbType.String);
                    param.Add("@Controller", error.Controller, DbType.String);
                    param.Add("@Method", error.Method, DbType.String);
                    param.Add("@InputValue", error.InputValue, DbType.String);
                    param.Add("@Message", error.Message, DbType.String);
                    param.Add("@InnerException", error.InnerException, DbType.String);
                    param.Add("@StackTrace", error.StackTrace, DbType.String);
                    param.Add("@InsertedBy", error.InsertedBy, DbType.String);

                    var data = conn.QueryAsync<int>(Constants.StoredProcedure.CREATELOG, param, null, null, CommandType.StoredProcedure);
                    //int result = 0;

                    //if (data.ToList().Count > 0)
                    //    result = data.ToList()[0];

                    //return result;
                }
            }
            catch (Exception ex)
            {
                //return 0;
            }
        }

        public async Task<int> LogErrorAsync(ErrorLog error)
        {
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@ErrorCode", error.ErrorCode, DbType.String);
                    param.Add("@Controller", error.Controller, DbType.String);
                    param.Add("@Method", error.Method, DbType.String);
                    param.Add("@InputValue", error.InputValue, DbType.String);
                    param.Add("@Message", error.Message, DbType.String);
                    param.Add("@InnerException", error.InnerException, DbType.String);
                    param.Add("@StackTrace", error.StackTrace, DbType.String);
                    param.Add("@InsertedBy", error.InsertedBy, DbType.String);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.CREATELOG, param, null, null, CommandType.StoredProcedure);
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

        public async Task<string> GetErrors(int record)
        {
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@record", record, DbType.Int32);

                    dynamic data = await conn.QueryAsync<ErrorLog>(@"with cte as (
	                        select ROW_NUMBER() over(order by ErrorLogId DESC) AS Sort, ErrorLogId, Controller, Method, InputValue, InnerException, [Message], StackTrace, InsertedBy, InsertedOn from ErrorLog)
                        select ErrorLogId, Controller, Method, InputValue, InnerException, [Message], StackTrace, InsertedBy, InsertedOn from cte where Sort <= @record", param, null, null, CommandType.Text);

                    return JsonConvert.SerializeObject((List<ErrorLog>)data).ToString();
                }
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex).ToString();
            }
        }
    }
}