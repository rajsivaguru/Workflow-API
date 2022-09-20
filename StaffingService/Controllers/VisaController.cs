using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StaffingService.Controllers
{
    [CustomExceptionFilter]
    public class VisaController : BaseController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetScreenQuestions()
        {
            ResponseModel result = await VisaDal.Instance.GetScreeQuestions();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetNoteQuestions(int loginId)
        {
            ResponseModel result = await VisaDal.Instance.GetNoteQuestions();
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveScreenQuestion(ScreenQuestion source)
        {
            ResponseModel result = await VisaDal.Instance.SaveScreenQuestion(source, source.loginid);
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveScreening(ScreenedProcess source)
        {
            ResponseModel result = await VisaDal.Instance.SaveScreening(source, source.loginid);
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> DeleteQuestion(int questionId, int loginId)
        {
            ResponseModel result = await VisaDal.Instance.DeleteQuestion(questionId, loginId);
            return SendResult(result);
        }

        private HttpResponseMessage sendResult(ResponseModel result)
        {
            ////if (result.ResultStatus == 1)
            return Request.CreateResponse(HttpStatusCode.OK, result);
            ////else
            ////    return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        }

        #region Unused old codes

        [HttpGet]
        public async Task<HttpResponseMessage> GetCategories()
        {
            ResponseModel result = await VisaDal.Instance.GetCategories();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetQuestions()
        {
            ResponseModel result = await VisaDal.Instance.GetQuestions();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetExternalConsultants()
        {
            ResponseModel result = await VisaDal.Instance.GetExternalConsultants();
            return SendResult(result);
        }

        ////[HttpPost]
        ////public async Task<HttpResponseMessage> SaveQuestion(Question source, int loginId)
        ////{
        ////    ResponseModel result = await VisaDal.Instance.SaveQuestion(source, loginId);
        ////    return sendResult(result);
        ////}

        [HttpPost]
        public async Task<HttpResponseMessage> SaveQuestion(Question source)
        {
            ResponseModel result = await VisaDal.Instance.SaveQuestion(source, 1);
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveExternalConsultant(ExternalConsultant source)
        {
            ResponseModel result = await VisaDal.Instance.SaveExternalConsultant(source, 1);
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> DeleteExternalConsultant(int consultantId, int loginId)
        {
            ResponseModel result = await VisaDal.Instance.DeleteExternalConsultant(consultantId, loginId);
            return SendResult(result);
        }


        #endregion
    }
}