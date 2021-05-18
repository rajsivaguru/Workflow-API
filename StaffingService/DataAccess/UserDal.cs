using Dapper;
using Newtonsoft.Json;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using WorkFlow.DataAccess;

namespace StaffingService.DataAccess
{
    internal class UserDal
    {
        private static ISqlDbConnection _dbConnection;

        private static UserDal instance = null;
        private static readonly object padlock = new object();

        private UserDal()
        {
        }
        
        public static UserDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new UserDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }
        
        public async Task<User> GetUser(string email)
        {
            try
            {
                UserDto dto = null;
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@email", email, DbType.String);
                    
                    var data = await conn.QueryAsync<UserDto>(Constants.StoredProcedure.GETUSER, param, null, null, CommandType.StoredProcedure);
                    if (data.ToList().Count > 0)
                    {
                        dto = data.ToList()[0];
                    }
                }

                if(dto != null)
                {
                    User user = new User()
                    {
                        userid = dto.userid,
                        email = dto.email,
                        name = Common.GetFullName(dto.fname, dto.minitial, dto.lname, dto.email),
                        roleid = dto.roleid,
                        rolename = dto.rolename,
                        workphone = dto.workphone,
                        mobile = dto.mobile,
                        homephone = dto.homephone,
                        location = dto.location,
                        imgurl = dto.imgurl,
                        status = dto.status
                    };
                    return user;
                }
                else
                {
                    return new User() { userid = 0 };
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog()
                {
                    Controller = "User",
                    Method = "Login",
                    InnerException = ex.InnerException == null ? string.Empty : ex.InnerException.ToString(),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InsertedBy = email,
                    InputValue = $"email:{email}"
                };

                LogDal.Instance.LogError(log);
            }

            return new User() { userid = -1 };
        }

        public async Task<User> Sync_GetUserDetail(User source)
        {
            UserDto dto = null;
            User user = null;
            List<string> roles = new List<string>();

            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (IDbConnection conn = _dbConnection.Connection)
                    {
                        DynamicParameters param = new DynamicParameters();

                        param.Add("@email", source.email, DbType.String);
                        param.Add("@img", source.imgurl, DbType.String);
                        param.Add("@fname", source.firstname, DbType.String);
                        param.Add("@lname", source.lastname, DbType.String);
                        
                        var data = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETUSER, param, null, null, CommandType.StoredProcedure);
                        var userdetail = data.Read<UserDto>().ToList();
                        roles = data.Read<string>().ToList();

                        if (userdetail.Count > 0)
                            dto = userdetail[0];
                    }
                    
                    if (dto != null)
                    {
                        user = new User()
                        {
                            userid = dto.userid,
                            email = dto.email,
                            name = Common.GetFullName(dto.fname, dto.minitial, dto.lname, dto.email),
                            roleid = dto.roleid,
                            rolename = dto.rolename,
                            workphone = dto.workphone,
                            mobile = dto.mobile,
                            homephone = dto.homephone,
                            location = dto.location,
                            imgurl = dto.imgurl,
                            status = dto.status
                        };
                        if (roles.Count > 0)
                            user.rolenames = roles;
                    }
                    else
                    {
                        new User() { userid = 0 };
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog()
                {
                    Controller = "User",
                    Method = "AuthUser",
                    InnerException = ex.InnerException == null ? string.Empty : ex.InnerException.ToString(),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InsertedBy = source.email,
                    InputValue = $"email:{source.email}"
                };

                LogDal.Instance.LogError(log);
            }

            if (user.userid >= 0)
                return user;
            return new User() { userid = -1 };
        }

        public async Task<string> GetAllUsers(int statusId)
        {
            try
            {
                //using (IDbConnection conn = _connection)
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();
                    
                    param.Add("@statusId", statusId, DbType.Int32);

                    dynamic data = await conn.QueryAsync<UserDto>(Constants.StoredProcedure.GETALLUSERS, param, null, null, CommandType.StoredProcedure);

                    List<User> users = new List<User>();
                    ((List<UserDto>)data).ToList().ForEach((x) =>
                    {                        
                        users.Add(new User()
                        {
                            userid = x.userid,
                            email = x.email,
                            name = Common.GetFullName(x.fname, x.minitial, x.lname, x.email),
                            roleid = x.roleid,
                            rolename = x.rolename,
                            workphone = x.workphone,
                            mobile = x.mobile,
                            homephone = x.homephone,
                            location = x.location,
                            imgurl = x.imgurl,
                            status = x.status
                        });
                    });
                    
                    return JsonConvert.SerializeObject(users).ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }
        
        public async Task<ResponseModel> GetUsersForAssignment(int statusId)
        {
            ResponseModel response = new ResponseModel();
            List<UserAssignment> users = new List<UserAssignment>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                dynamic data = await conn.QueryAsync<UserAssignmentDto>(Constants.StoredProcedure.GETASSIGNMENTUSERS, null, null, null, CommandType.StoredProcedure);


                ((List<UserAssignmentDto>)data).ToList().ForEach((x) =>
                {
                    users.Add(new UserAssignment()
                    {
                        userid = x.userid,
                        email = x.email,
                        name = $"{Common.GetFullName(x.fname, x.minitial, x.lname, x.email)} ({x.jobsassigned})",
                        roleid = x.roleid,
                        rolename = x.rolename,
                        imgurl = x.imgurl,
                        jobsassigned = x.jobsassigned
                    });
                });

                //return JsonConvert.SerializeObject(users).ToString();

                response = Common.GetResponse(users.Count);
                response.Output = users;
            }

            return response;
        }

        public async Task<string> GetAbsentUsers()
        {
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    dynamic data = await conn.QueryAsync<UserAssignmentDto>(Constants.StoredProcedure.GETABSENTUSERS, null, null, null, CommandType.StoredProcedure);

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
                            imgurl = x.imgurl,
                            jobsassigned = x.jobsassigned,
                            isabsent = x.isabsent
                        });
                    });

                    return JsonConvert.SerializeObject(users).ToString();
                }
            }
            catch (Exception ex)
            {
            }
            return string.Empty;
        }

        public async Task<string> SaveAbsentUsers(string userIds, int loginId)
        {
            string message = string.Empty;
            try
            {
                using (IDbConnection conn = _dbConnection.Connection)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@UserIds", userIds, DbType.String);
                    param.Add("@LoginId", loginId, DbType.Int32);

                    dynamic data = await conn.QueryAsync<int>(Constants.StoredProcedure.SAVEABSENTUSERS, param, null, null, CommandType.StoredProcedure);
                    int res = 0;
                    
                    if (((List<int>)data).ToList().Count > 0)
                        res = ((List<int>)data).ToList()[0];

                    message = res == 0 ? "Error occurred while saving Absent User(s).  Please try again." : "Absent user(s) saved successfully.";
                }
            }
            catch (Exception ex)
            {
                message = "Error occurred while saving Absent User(s).";
            }

            return JsonConvert.SerializeObject(message).ToString();
        }

        public async Task<int> SyncUserAfterLogin(User user)
        {
            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@email", user.email, DbType.String);
                param.Add("@img", user.imgurl, DbType.String);
                param.Add("@fname", user.firstname, DbType.String);
                param.Add("@lname", user.lastname, DbType.String);

                dynamic data = await conn.QueryAsync<int>(Constants.StoredProcedure.SYNCUSERAFTERLOGIN, param, null, null, CommandType.StoredProcedure);
                return data[0];
            }
        }
    }
}