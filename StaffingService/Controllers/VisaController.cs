using StaffingService.DataAccess;
using StaffingService.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace StaffingService.Controllers
{
    public class VisaController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetScreenQuestions()
        {
            ResponseModel result = await VisaDal.Instance.GetScreeQuestions();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetNoteQuestions(int loginId)
        {
            ResponseModel result = await VisaDal.Instance.GetNoteQuestions();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveScreenQuestion(ScreenQuestion source)
        {
            ResponseModel result = await VisaDal.Instance.SaveScreenQuestion(source, source.loginid);
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveScreening(ScreenedProcess source)
        {
            ResponseModel result = await VisaDal.Instance.SaveScreening(source, source.loginid);
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> DeleteQuestion(int questionId, int loginId)
        {
            ResponseModel result = await VisaDal.Instance.DeleteQuestion(questionId, loginId);
            return sendResult(result);
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
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetQuestions()
        {
            ResponseModel result = await VisaDal.Instance.GetQuestions();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetExternalConsultants()
        {
            ResponseModel result = await VisaDal.Instance.GetExternalConsultants();
            return Request.CreateResponse(HttpStatusCode.OK, result);
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
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveExternalConsultant(ExternalConsultant source)
        {
            ResponseModel result = await VisaDal.Instance.SaveExternalConsultant(source, 1);
            return sendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> DeleteExternalConsultant(int consultantId, int loginId)
        {
            ResponseModel result = await VisaDal.Instance.DeleteExternalConsultant(consultantId, loginId);
            return sendResult(result);
        }


        #endregion
    }
}