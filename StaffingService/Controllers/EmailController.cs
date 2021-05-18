using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
//using System.Web.Mvc;

namespace StaffingService.Controllers
{
    [CustomExceptionFilter]
    [System.Web.Mvc.RoutePrefix("Email")]
    public class EmailController : ApiController
    {
        [HttpGet]
        public async Task<HttpResponseMessage> GetEmailDetails()
        {
            ResponseModel result = await EmailDal.Instance.GetEmailDetails();
            return sendResult(result);
        }


        [System.Web.Http.HttpPost]
        public async Task<HttpResponseMessage> SendEmail(ComposeEmailParam source)
        {
            ResponseModel result = new ResponseModel();
            
            if (source.toaddresses != null && source.toaddresses.Count > 0)
            {
                Attachment att = null;
                MailMessage message = new MailMessage();
                string fromAddress = "";

                EmailConfigDetails config = await EmailDal.Instance.GetEmailConfigDetails();
                SmtpClient smtp = new SmtpClient(config.Server, config.Port);

                if (!source.sendseperateemail)
                    message.To.Add(string.Join(",", source.toaddresses));

                if (source.usedefaultfromaddress)
                    fromAddress = config.DefaultFromAddress;
                else
                    fromAddress = source.fromaddress;

                fromAddress = config.DefaultFromAddress;

                message.From = new MailAddress(fromAddress, config.DefaultFromAddress);
                message.Subject = source.subject;
                message.Body = source.body;
                message.IsBodyHtml = true;

                if (source.ccaddresses != null && source.ccaddresses.Count > 0)
                {
                    string ccAddress = string.Join(",", source.ccaddresses);
                    message.CC.Add(ccAddress);
                }
                if (source.bccaddresses != null && source.bccaddresses.Count > 0)
                    message.Bcc.Add(string.Join(",", source.bccaddresses));
                                
                ////if (!String.IsNullOrEmpty(AttachmentFile))
                ////{
                ////    if (File.Exists(AttachmentFile))
                ////    {
                ////        att = new Attachment(AttachmentFile);
                ////        message.Attachments.Add(att);
                ////    }
                ////}
                if (config.AuthenticateUser.Trim().Length > 0 && config.AuthenticatePassword.Trim().Length > 0)
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(config.AuthenticateUser.Trim(), config.AuthenticatePassword.Trim());
                    smtp.EnableSsl = config.EnableSSL;
                }                
                //else
                //{
                //    smtp.UseDefaultCredentials = true;
                //}

                //foreach (var item in Recipient.Split(','))
                //{
                //    try
                //    {
                //        message.To.Add(item);
                //    }
                //    catch
                //    { }
                //}

                if (!source.sendseperateemail)
                    smtp.Send(message);
                else
                {
                    for(int i = 0; i < source.toaddresses.Count; i++)
                    {
                        message.To.Clear();
                        message.To.Add(source.toaddresses[i]);
                        smtp.Send(message);
                    }
                }

                if (att != null)
                    att.Dispose();
                message.Dispose();
                smtp.Dispose();

                result.ResultStatus = 1;
                result.SuccessMessage = "Email sent successfully.";
            }
            else
            {
                result.SuccessMessage = "Email not sent.";
            }
            
            return sendResult(result);
        }

        private HttpResponseMessage sendResult(ResponseModel result)
        {
            if (result.ResultStatus == 1)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            else
                return Request.CreateResponse(HttpStatusCode.InternalServerError, result);
        }
    }
}