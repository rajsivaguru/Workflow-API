using StaffingService.DataAccess;
using System.Threading.Tasks;
using System.Web.Http;

namespace StaffingService.Controllers
{
    public class ErrorController : ApiController
    {   
        [HttpGet]
        public async Task<string> GetErrors(int record)
        {
            return await LogDal.Instance.GetErrors(record);
        }
    }
}
