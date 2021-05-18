using Newtonsoft.Json;
using StaffingService.DataAccess;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StaffingService.Controllers
{
    public class UserController : BaseController
    {
        public UserController()
        {
        }
        
        [HttpGet]
        public async Task<string> SyncUser(string email, string imgurl, string firstname, string lastname, string name)
        {
            var jsonResult = new
            {
                Result = "-1",
                Message = "Error occurred, Please try again."
            };

            User user = await UserDal.Instance.Sync_GetUserDetail(new User() { email = email, imgurl = imgurl, firstname = firstname, lastname = lastname });

            if (user != null && user.userid > 0)
            {
                string toke = Guid.NewGuid().ToString();
                user.token = toke;

                UserCache item = new UserCache()
                {
                    Token = toke,
                    Email = email,
                    UserId = user.userid,
                    LastAccessedOn = DateTime.Now,
                    UserName = user.name ?? name,
                    Roles = user.rolenames
                };
                CacheManager.PostUserCache(email, item);

                jsonResult = new
                {
                    Result = "1",
                    Message = JsonConvert.SerializeObject(user).ToString()
                };
                return JsonConvert.SerializeObject(jsonResult).ToString();
            }
            else if (user != null && user.userid == 0)
            {
                jsonResult = new
                {
                    Result = "0",
                    Message = "User not authorized."
                };
                return JsonConvert.SerializeObject(jsonResult).ToString();
            }
            return JsonConvert.SerializeObject(jsonResult).ToString();
        }

        [HttpGet]
        public async Task<string> GetAllUsers(int statusId, int loginId)
        {
            return await UserDal.Instance.GetAllUsers(statusId);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetUsersForAssignment(int statusId, int loginId)
        {
            ResponseModel result = await UserDal.Instance.GetUsersForAssignment(statusId);
            return SendResult(result);
        }

        [HttpGet]
        public async Task<string> GetAbsentUsers(int loginId)
        {
            return await UserDal.Instance.GetAbsentUsers();
        }

        [HttpGet]
        public async Task<string> SaveAbsentUsers(string userIds, int loginId)
        {
            /* userIds = comma separated userid. */
            return await UserDal.Instance.SaveAbsentUsers(userIds, loginId);
        }

        [HttpPost]
        public string SaveAbsentUsers([FromBody] string userIds)
        {
            /* userIds = comma separated userid. */
            return "success";
        }

        [HttpPost]
        public string SaveAbsentUsersO(Test test)
        {
            /* userIds = comma separated userid. */
            return "success";
        }
        ////[HttpPost]
        ////public string SaveAbsentUsers()
        ////{
        ////    /* userIds = comma separated userid. */
        ////    return "empty success";
        ////}        
    }

    public class Test
    {
        public string name { get; set; }
        public string name2 { get; set; }
    }
}
