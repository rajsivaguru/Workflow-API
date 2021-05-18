using StaffingService.DataAccess;
using StaffingService.Filters;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static StaffingService.Models.Accounts;

namespace StaffingService.Controllers
{
    [UserAuthorizationFilter]
    [CustomExceptionFilter]
    public class InvoiceController : BaseController
    {
        [HttpGet]
        public HttpResponseMessage GetCache()
        {
            ResponseModel result = new ResponseModel()
            {
                ResultStatus = Constants.ResponseResult.SUCCESS,
                OutputCount = 1
            };
            result.Output = CacheManager.GetUserCache("rajsivaguru@gmail.com");
            return SendResult(result);
        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetInvoiceSupportingDetails()
        {
            ResponseModel result = await InvoiceDal.Instance.GetInvoiceSupportingDetails();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetConsultants()
        {
            ResponseModel result = await InvoiceDal.Instance.GetConsultants();
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetActiveInvoices(bool generateInvoice = false)
        {
            int? loginId = Common.GetLoginId(Request.Headers);

            if (!loginId.HasValue)
                loginId = 0;

            ResponseModel result = await InvoiceDal.Instance.GetActiveInvoices(Convert.ToInt32(loginId), generateInvoice);
            return SendResult(result);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetInvoiceDetail(int invoiceId)
        {
            ResponseModel result = await InvoiceDal.Instance.GetInvoiceDetail(invoiceId);
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveConsultant(Consultant source)
        {
            ResponseModel result = null;
            if (source.consultantid > 0)
            {
                if(source.enddate >= source.invoicestartdate)
                {
                    int? loginId = Common.GetLoginId(Request.Headers);
                    result = await InvoiceDal.Instance.EndConsultant(source, Convert.ToInt32(loginId));
                }
            }
            else if (source.consultantid == 0)
            {
                if (ModelState.IsValid)
                {
                    int? loginId = Common.GetLoginId(Request.Headers);
                    result = await InvoiceDal.Instance.SaveConsultant(source, Convert.ToInt32(loginId));
                }
                else
                {
                    if(ModelState.Count > 0)
                    {
                        if(ModelState.Values.ElementAt(0).Errors.Count == 1)
                            result = new ResponseModel()
                            {
                                ErrorMessage = ModelState.Values.ElementAt(0).Errors[0].ErrorMessage,
                                SuccessMessage = string.Empty,
                                ResultStatus = Constants.ResponseResult.MISSINGDATA
                            };
                    }
                }
            }
            return SendResult(result);
        }

        [HttpPost]
        public async Task<HttpResponseMessage> SaveInvoice(Invoice source)
        {
            ResponseModel result = null;
            bool isModelValid = false;

            isModelValid = validateInvoiceSource(source);

            if (source.invoiceid > 0 && isModelValid)
            {
                int? loginId = Common.GetLoginId(Request.Headers);
                result = await InvoiceDal.Instance.SaveInvoice(source, Convert.ToInt32(loginId));
            }
            else
            {
                result = new ResponseModel()
                {
                    ErrorMessage = Constants.ErrorMessage.MISSINGDATA,
                    SuccessMessage = string.Empty,
                    ResultStatus = Constants.ResponseResult.MISSINGDATA,
                    RequestType = Constants.RequestType.POST
                };
            }
            return SendResult(result);
        }

        private bool validateInvoiceSource(Invoice source)
        {
            bool isModelValid = false;

            switch (source.status)
            {
                case "Timesheet Received/Verified":
                    if (source.tsreceiveddate != null && source.tsreceiveddate <= DateTime.Now && source.tsactualhours != null && source.tsactualhours > 0)
                        isModelValid = true;
                    break;
                case "Invoice Created":
                    if (source.invoicecreateddate != null && source.invoicecreateddate <= DateTime.Now && source.invoiceamount > 0 && !string.IsNullOrWhiteSpace(source.invoicenumber))
                        isModelValid = true;
                    break;
                case "Invoice Sent":
                    if (source.invoicesentdate != null && source.invoicesentdate <= DateTime.Now)
                        isModelValid = true;
                    break;
                case "Partial Payment Received":
                case "Payment Received":
                    if (source.paymentreceiveddate != null && source.paymentreceiveddate <= DateTime.Now && source.paymentamount > 0)
                        isModelValid = true;
                    break;
                case "Bill Paid":
                    if (source.billpaiddate != null && source.billpaiddate <= DateTime.Now && source.billamount > 0 && source.billhours > 0)
                        isModelValid = true;
                    break;
                case "Commission Paid":
                    if (source.commissionpaiddate != null && source.commissionpaiddate <= DateTime.Now && source.commissionamount > 0 && source.commissionhours > 0)
                        isModelValid = true;
                    break;
                case "Complete Invoice":
                    isModelValid = true;
                    break;
            }

            return isModelValid;
        }
    }
}