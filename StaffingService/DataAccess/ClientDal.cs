using Dapper;
using Newtonsoft.Json;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace StaffingService.DataAccess
{
    public class ClientDal
    {
        private static ISqlDbConnection _dbConnection;
        private static ClientDal instance = null;
        private static readonly object padlock = new object();

        private ClientDal()
        {
        }
        
        public static ClientDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ClientDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }

        public async Task<ResponseModel> GetClients()
        {
            ResponseModel response = new ResponseModel();
            List<Client> result = new List<Client>();
            
            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<Client>(Constants.StoredProcedure.GETCLIENT, null, null, null, CommandType.StoredProcedure);
                result = (List<Client>)data;

                ////return JsonConvert.SerializeObject((List<Client>)data).ToString();
                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        public async Task<string> SaveClient(Client source, int loginUserId)
        {
            var jsonResult = new
            {
                Result = "0",
                Message = "Client not saved, Please try again."
            };
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    if(source.id > 0)
                        param.Add("@Id", source.id, DbType.Int32);

                    param.Add("@Name", source.clientname.Trim(), DbType.String);
                    param.Add("@ShortName", source.shortname.Trim(), DbType.String);
                    param.Add("@LoginUserId", loginUserId, DbType.Int32);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGECLIENT, param, null, null, CommandType.StoredProcedure);
                    int res = 0;

                    if (((List<int>)data).ToList().Count > 0)
                        res = ((List<int>)data).ToList()[0];

                    jsonResult = new
                    {
                        Result = res.ToString(),
                        Message = res == 0 ? "Error occurred while saving client.  Please try again." : "Client saved successfully."
                    };

                    return JsonConvert.SerializeObject(jsonResult).ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return JsonConvert.SerializeObject(jsonResult);
        }

        public async Task<string> DeleteClient(int id, int loginUserId)
        {
            var jsonResult = new
            {
                Result = "0",
                Message = "Client not deleted, Please try again."
            };
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@Id", id, DbType.Int32);
                    param.Add("@IsDeletable", true, DbType.Boolean);
                    param.Add("@LoginUserId", loginUserId, DbType.Int32);

                    var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGECLIENT, param, null, null, CommandType.StoredProcedure);
                    int res = 0;

                    if (((List<int>)data).ToList().Count > 0)
                        res = ((List<int>)data).ToList()[0];

                    jsonResult = new
                    {
                        Result = res.ToString(),
                        Message = res == 0 ? "Error occurred while deleting client.  Please try again." : "Client deleted successfully."
                    };

                    return JsonConvert.SerializeObject(jsonResult).ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return JsonConvert.SerializeObject(jsonResult);
        }
    }
}