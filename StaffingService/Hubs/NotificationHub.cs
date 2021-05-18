using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using StaffingService.Util;
using StaffingService.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace StaffingService.Hubs
{
    public class NotificationHub : Hub
    {
        private static IHubContext hub = GlobalHost.ConnectionManager.GetHubContext("NotificationHub");
                
        public override async Task OnConnected()
        {
            await hub.Clients.All.onConnected("Connected");
            await base.OnConnected();
        }

        public override async Task OnReconnected()
        {
            await hub.Clients.All.onReconnected("ReConnected");
            await base.OnReconnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await hub.Clients.All.onDisconnected("DisConnected");
            await base.OnDisconnected(stopCalled);
        }

        public void Hello()
        {
            Clients.All.hello();
        }

        public void NotifyAll(string from, string message)
        {
            hub.Clients.All.notifyAll(from, message);
        }
        
        public void UpdateCache(string token, string connectionId)
        {
            CacheManager.PutSignalRConnectionCache(token, connectionId);
        }

        public void NotifyJobInterest(Job job, UserCache user, int notificationId, string message = "")
        {
            Notification notification = new Notification()
            {
                notificationid = notificationId,
                notificationtype = "Job Interest",
                createdon = DateTime.Now.ToString(),
                fromuser = user.UserName,
                isread = false,
                jobid = job.jobid,
                messagetext = string.IsNullOrWhiteSpace(message) ? $"{user.UserName} is interested in the job ({job.title} - {job.referenceid})." : message,
                referenceid = job.referenceid,
                title = job.title
            };

            string toNotify = JsonConvert.SerializeObject(notification);

            List<string> connections = CacheManager.GetConnectionIdsForSU_DM_TL();
            ////hub.Clients.All.notifyJobInterest(toNotify);
            hub.Clients.Clients(connections).notifyJobInterest(toNotify);
        }
    }
}