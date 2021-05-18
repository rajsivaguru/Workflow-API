using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StaffingService.Controllers
{
    [UserAuthorizationFilter]
    [CustomExceptionFilter]
    public class ClientController : BaseController
    {
        public ClientController()
        {
        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetClients()
        {
            ResponseModel result = await ClientDal.Instance.GetClients();
            return SendResult(result);
        }

        [HttpPost]
        public async Task<string> SaveClient(Client source)
        {
            int? loginId = Common.GetLoginId(Request.Headers);
            return await ClientDal.Instance.SaveClient(source, Convert.ToInt32(loginId));
        }

        [HttpGet]
        public async Task<string> DeleteClient(int clientId)
        {
            int? loginId = Common.GetLoginId(Request.Headers);
            return await ClientDal.Instance.DeleteClient(clientId, Convert.ToInt32(loginId));
        }
    }
}