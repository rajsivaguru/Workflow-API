using StaffingService.DataAccess;
using StaffingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace StaffingService.Util
{
    internal static class Common
    {
        internal static string GetFullName(string firstname, string mname, string lastname, string email)
        {
            if (string.IsNullOrWhiteSpace(firstname))
                return email;
            else if (string.IsNullOrWhiteSpace(mname))
                return $"{firstname} {lastname}";
            else 
                return $"{firstname} {mname} {lastname}";
        }

        internal static DataTable ConvertListToDataTable(List<UserReport> list)
        {
            // New table.
            DataTable table = new DataTable();

            // Get max columns.
            int columns = 13;

            // Add columns.
            for (int i = 0; i < columns; i++)
            {
                table.Columns.Add();
            }

            // Add rows.
            foreach (var array in list)
            {
                table.Rows.Add(array);
            }

            return table;
        }

        internal static ResponseModel GetResponse(int result)
        {
            ResponseModel response = new ResponseModel()
            {
                ResultStatus = result > 0 ? Constants.ResponseResult.SUCCESS : Constants.ResponseResult.NODATA,
                OutputCount = result
            };
            return response;
        }

        internal static void LogErrorInDB(ErrorLog error)
        {
            LogDal.Instance.LogError(error);
        }

        internal static async Task<int> LogErrorInDBAsync(ErrorLog error)
        {
            return await LogDal.Instance.LogErrorAsync(error);
        }

        internal static string GetUIToken(object source)
        {
            string token = string.Empty;            
            Type type = source.GetType();

            switch (type.Name.ToString())
            {
                case Constants.ObjectType.HTTPREQUESTHEADERS:
                    HttpHeaders headers = ((HttpHeaders)source);
                    if (headers.Contains("Authorization"))
                    {
                        string auth = headers.GetValues("Authorization").FirstOrDefault();
                        token = auth.Substring(auth.IndexOf(" ") + 1);
                    }
                    break;

                default:
                    break;
            }

            return token;
        }
        
        internal static int? GetLoginId(object source)
        {
            string token = GetUIToken(source);

            if(!string.IsNullOrWhiteSpace(token))
            {
                var user = CacheManager.GetUserCacheByToken(token);
                return user.UserId;
            }
            return null;
        }

        internal static UserCache GetUser(object source)
        {
            string token = GetUIToken(source);

            if (!string.IsNullOrWhiteSpace(token))
            {
                var user = CacheManager.GetUserCacheByToken(token);
                return user;
            }
            return null;
        }

        internal static bool IsUITokenValid(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                string userCacheToken = CacheManager.GetUserToken(token);

                if (!string.IsNullOrWhiteSpace(userCacheToken))
                    return true;
            }

            return false;
        }
        
    }
}