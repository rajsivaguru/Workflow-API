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
    public class CommonController : BaseController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetStatuses(string type = "")
        {
            ResponseModel result = await CommonDal.Instance.GetStatus(type);
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetCustomerTypes()
        {
            ResponseModel result = await CommonDal.Instance.GetCustomerTypes();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetCustomersVendors()
        {
            ResponseModel result = await CommonDal.Instance.GetCustomersVendors();
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveCustomerVendor(CustomerVendor source)
        {
            ResponseModel result = null;
            int? loginId = Common.GetLoginId(Request.Headers);
            result = await CommonDal.Instance.SaveCustomerVendor(source, Convert.ToInt32(loginId));
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> DeleteCustomerVendor(int id)
        {
            ResponseModel result = null;
            int? loginId = Common.GetLoginId(Request.Headers);
            result = await CommonDal.Instance.DeleteCustomerVendor(id, Convert.ToInt32(loginId));
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetMyNotifications()
        {
            ResponseModel result = null;
            int? loginId = Common.GetLoginId(Request.Headers);
            result = await CommonDal.Instance.GetMyNotifications(Convert.ToInt32(loginId));
            return SendResult(result);
        }

    }
}