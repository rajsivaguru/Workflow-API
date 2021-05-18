using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaffingService.Util
{
    public class UserCache
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string SignalRConnectionId { get; set; }
        public DateTime LastAccessedOn { get; set; }

        public List<string> Roles { get; set; }
    }

    public class CacheManager
    {
        static Dictionary<string, UserCache> userCache;
        static List<string> userTokens;

        static CacheManager()
        {
            userCache = new Dictionary<string, UserCache>();
            userTokens = new List<string>();
        }

        /* Return the UserInfo for the requested userid if any.  Else return null. */
        internal static UserCache GetUserCache(string email)
        {
            if (userCache.ContainsKey(email))
            {
                return userCache[email];
            }
            else
                return null;
        }

        /* Return the UserInfo for the requested userid if any.  Else return null. */
        internal static UserCache GetUserCacheByToken(string token)
        {
            UserCache user = null;
            userCache.ToList().ForEach((x) =>
            {
                if (x.Value.Token == token)
                    user = x.Value;
            });
            return user;
        }

        internal static string GetUserToken(string token)
        {
            if (userTokens.Contains(token))
            {
                return token;
            }
            else
                return null;
        }
        
        internal static List<string> GetConnectionIdsForSU_DM_TL()
        {
            try
            {
                List<string> connectionIds = userCache.ToList().Where(x => x.Value.Roles.Contains("Super User") || x.Value.Roles.Contains("Delivery Manager") || x.Value.Roles.Contains("Team Lead")).Select(x => x.Value.SignalRConnectionId).ToList();
                return connectionIds;
            }
            catch(Exception ex)
            {
                return new List<string>();
            }
        }

        /* Add the user to the cache. */
        internal static void PostUserCache(string email, UserCache userData)
        {
            /* Ensure whether this contains data.
             * If not continue and add the info. */
            if (!userCache.ContainsKey(email))
            {
                userCache.Add(email, userData);
                userTokens.Add(userData.Token);
            }
            else
            {
                var user = GetUserCache(email);

                if(user != null && !string.IsNullOrWhiteSpace(user.Token))
                    userTokens.Remove(user.Token);

                userCache.Remove(email);
                userTokens.Add(userData.Token);
                userCache.Add(email, userData);
            }
        }
        

        /* Update the LastAccessed time to the cache. */
        internal static void PutTimeCache(string userid)
        {
            if (userCache.ContainsKey(userid))
                (userCache[userid]).LastAccessedOn = DateTime.Now;
        }

        /* Update the SignalR connectionId to the cache. */
        internal static void PutSignalRConnectionCache(string token, string connectionId)
        {
            UserCache user = null;
            userCache.ToList().ForEach((x) =>
            {
                if (x.Value.Token == token)
                    user = x.Value;
            });
            
            if (userCache.ContainsKey(user.Email))
                (userCache[user.Email]).SignalRConnectionId = connectionId;
        }
    }
}
