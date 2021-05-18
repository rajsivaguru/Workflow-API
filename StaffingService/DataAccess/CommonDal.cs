using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffingService.DataAccess
{
    public class CommonDal
    {
        private static ISqlDbConnection _dbConnection;
        private static CommonDal instance = null;
        private static readonly object padlock = new object();

        public CommonDal()
        {
        }

        public static CommonDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CommonDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }

        internal async Task<ResponseModel> GetStatus(string type)
        {
            ResponseModel response = new ResponseModel();
            List<Status> result = new List<Status>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@type", type, DbType.String);

                var data = await conn.QueryAsync<Status>(Constants.StoredProcedure.GETSTATUS, param, null, null, CommandType.StoredProcedure);
                result = (List<Status>)data;
                
                response = Common.GetResponse(data.Count());
                response.Output = result;
            }
            
            return response;
        }

        internal async Task<ResponseModel> GetCustomerTypes()
        {
            ResponseModel response = new ResponseModel();
            List<CustomerType> result = new List<CustomerType>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<CustomerType>(Constants.StoredProcedure.GETCUSTOMERTYPES, null, null, null, CommandType.StoredProcedure);
                result = (List<CustomerType>)data;

                response = Common.GetResponse(data.Count());
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetCustomersVendors()
        {
            ResponseModel response = new ResponseModel();
            List<CustomerVendor> result = new List<CustomerVendor>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<CustomerVendor>(Constants.StoredProcedure.GETCUSTOMERS, null, null, null, CommandType.StoredProcedure);
                result = (List<CustomerVendor>)data;

                response = Common.GetResponse(data.Count());
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveCustomerVendor(CustomerVendor source, int loginUserId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                if (source.customervendorid > 0)
                    param.Add("@Id", source.customervendorid, DbType.Int32);

                param.Add("@Name", source.name.Trim(), DbType.String);
                param.Add("@Type", source.type.Trim(), DbType.String);
                param.Add("@LoginUserId", loginUserId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGECUSTOMERVENDOR, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Customer/Vendor saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> DeleteCustomerVendor(int id, int loginUserId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@Id", id, DbType.Int32);
                param.Add("@IsDeletable", true, DbType.Boolean);
                param.Add("@LoginUserId", loginUserId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGECUSTOMERVENDOR, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Customer/Vendor deleted successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while deleting.  Please try again." : string.Empty;
            }

            return response;
        }
        
        internal async Task<ResponseModel> GetMyNotifications(int loginUserId)
        {
            ResponseModel response = new ResponseModel();
            List<Notification> result = new List<Notification>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@LoginUserId", loginUserId, DbType.Int32);

                var data = await conn.QueryAsync<Notification>(Constants.StoredProcedure.GETMYNOTIFICATIONS, param, null, null, CommandType.StoredProcedure);
                result = (List<Notification>)data;

                response = Common.GetResponse(data.Count());
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetNotification(int notificationId)
        {
            ResponseModel response = new ResponseModel();
            List<Notification> result = new List<Notification>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@NotificationId", notificationId, DbType.Int32);

                var data = await conn.QueryAsync<Notification>(Constants.StoredProcedure.GETMYNOTIFICATIONS, param, null, null, CommandType.StoredProcedure);
                
                response.ResultStatus = 1;
                response.ErrorMessage = string.Empty;
                response.OutputCount = data.Count();
                response.Output = data;

                response = Common.GetResponse(data.Count());
                response.Output = result;
            }

            return response;
        }
    }
}