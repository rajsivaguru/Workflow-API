using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static StaffingService.Models.Config;
using static StaffingService.Models.Accounts;

namespace StaffingService.DataAccess
{
    public class InvoiceDal
    {
        private static ISqlDbConnection _dbConnection;
        private static InvoiceDal instance = null;
        private static readonly object padlock = new object();

        public InvoiceDal()
        {
        }

        public static InvoiceDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new InvoiceDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }

        internal async Task<ResponseModel> GetInvoiceSupportingDetails()
        {
            ResponseModel response = new ResponseModel();
            List<EmploymentType> types = new List<EmploymentType>();
            List<CommunicationMode> modes = new List<CommunicationMode>();
            List<BillingFrequency> frequencies = new List<BillingFrequency>();
            List<CustomerVendor> customervendors = new List<CustomerVendor>();
            List<InvoiceSupportingDetail> details = new List<InvoiceSupportingDetail>();
            
            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETINVOICESUPPORTDETAILS, null, null, null, CommandType.StoredProcedure);
                types = data.Read<EmploymentType>().ToList();
                modes = data.Read<CommunicationMode>().ToList();
                frequencies = data.Read<BillingFrequency>().ToList();
                customervendors = data.Read<CustomerVendor>().ToList();

                InvoiceSupportingDetail detail = new InvoiceSupportingDetail()
                {
                    employmenttypes = types,
                    billingfrequencies = frequencies,
                    communicationmodes = modes,
                    customervendors = customervendors
                };

                details.Add(detail);

                response = Common.GetResponse(details.Count);
                response.Output = details;
            }

            return response;
        }

        internal async Task<ResponseModel> GetConsultants()
        {
            ResponseModel response = new ResponseModel();
            List<Consultant> result = new List<Consultant>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<Consultant>(Constants.StoredProcedure.GETCONSULTANTS, null, null, null, CommandType.StoredProcedure);
                result = (List<Consultant>)data;
                
                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetActiveInvoices(int loginId, bool generateInvoice = false)
        {
            ResponseModel response = new ResponseModel();
            List<Invoice> result = new List<Invoice>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                if (loginId > 0)
                    param.Add("@LoginId", loginId, DbType.Int32);
                if (generateInvoice)
                    param.Add("@GenerateInvoice", generateInvoice, DbType.Boolean);

                var data = await conn.QueryAsync<Invoice>(Constants.StoredProcedure.GETACTIVEINVOICES, param, null, null, CommandType.StoredProcedure);
                result = (List<Invoice>)data;

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetInvoiceDetail(int invoiceId)
        {
            ResponseModel response = new ResponseModel();
            Invoice result = new Invoice();
            result.paymenthistory = new List<PaymentHistory>();
            List<PaymentHistory> payments = new List<PaymentHistory>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@InvoiceId", invoiceId, DbType.Int32);

                var data = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETINVOICEDETAILS, param, null, null, CommandType.StoredProcedure);
                var source = data.Read<Invoice>().ToList();
                payments = data.Read<PaymentHistory>().ToList();

                if (source.Count > 0)
                {
                    if (payments.Count > 0)
                        source[0].paymenthistory = payments;
                    else
                        source[0].paymenthistory = new List<PaymentHistory>();

                    result = source.ToList()[0];
                }

                response = Common.GetResponse(source.Count());
                response.Output = result; 
            }

            return response;
        }

        internal async Task<ResponseModel> SaveConsultant(Consultant source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@employmenttypeid", source.employmenttypeid, DbType.Int32);
                param.Add("@billingfrequencyid", source.billingfrequencyid, DbType.Int32);
                param.Add("@communicationid", source.communicationid, DbType.Int32);
                param.Add("@firstname", source.firstname, DbType.String);
                param.Add("@middleName", source.middlename, DbType.String);
                param.Add("@lastname", source.lastname, DbType.String);
                param.Add("@email", source.email, DbType.String);
                param.Add("@phone", source.phone, DbType.String);
                param.Add("@customer", source.customer, DbType.String);
                param.Add("@endclient", source.endclient, DbType.String);
                param.Add("@billpayto", source.billpayto, DbType.String);
                param.Add("@invoicestartdate", source.invoicestartdate.ToString("yyyy-MM-dd"), DbType.Date);
                param.Add("@startdate", source.startdate.ToString("yyyy-MM-dd"), DbType.Date);
                param.Add("@billrate", source.billrate, DbType.Decimal);
                param.Add("@payrate", source.payrate, DbType.Decimal);

                if (!string.IsNullOrWhiteSpace(source.commissionto))
                    param.Add("@commissionto", source.commissionto, DbType.String);
                if (source.commissionrate.HasValue)
                    param.Add("@commissionrate", source.commissionrate, DbType.Decimal);
                if (!string.IsNullOrWhiteSpace(source.invoiceemail))
                    param.Add("@CMEmail", source.invoiceemail, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.portalurl))
                    param.Add("@PortalURL", source.portalurl, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.portaluser))
                    param.Add("@PortalUser", source.portaluser, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.portalpwd))
                    param.Add("@PortalPwd", source.portalpwd, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.portalnotes))
                    param.Add("@PortalNotes", source.portalnotes, DbType.String);

                param.Add("@LoginId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SAVECONSULTANT, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Consultant saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> EndConsultant(Consultant source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ConsultantId", source.consultantid, DbType.Int32);
                param.Add("@EndDate", Convert.ToDateTime(source.enddate).ToString("yyyy-MM-dd"), DbType.Date);
                param.Add("@ShouldEndClient", true, DbType.Boolean);
                param.Add("@LoginId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.UPDATECONSULTANT, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Consultant saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveInvoice(Invoice source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@InvoiceId", source.invoiceid, DbType.Int32);
                param.Add("@StatusId", source.statusid, DbType.Int32);

                param.Add("@TSActualHours", source.tsactualhours, DbType.Decimal);
                param.Add("@TSReceivedDate", source.tsreceiveddate, DbType.Date);
                if (!string.IsNullOrWhiteSpace(source.tsnotes))
                    param.Add("@TSNotes", source.tsnotes, DbType.String);

                param.Add("@InvoiceCreatedDate", source.invoicecreateddate, DbType.Date);
                param.Add("@InvoiceAmount", source.invoiceamount, DbType.Decimal);
                param.Add("@InvoiceNumber", source.invoicenumber, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.invoicenotes))
                    param.Add("@InvoiceNotes", source.invoicenotes, DbType.String);
                
                param.Add("@InvoiceSentDate", source.invoicesentdate, DbType.Date);
                if (!string.IsNullOrWhiteSpace(source.invoicesentnotes))
                    param.Add("@InvoiceSentNotes", source.invoicesentnotes, DbType.String);
                
                param.Add("@PaymentReceivedDate", source.paymentreceiveddate, DbType.Date);
                param.Add("@PaymentAmount", source.paymentamount, DbType.Decimal);
                param.Add("@IsPartialPayment", source.ispartialpayment, DbType.Boolean);
                if (!string.IsNullOrWhiteSpace(source.paymentnotes))
                    param.Add("@PaymentNotes", source.paymentnotes, DbType.String);
                
                param.Add("@BillPaidDate", source.billpaiddate, DbType.Date);
                param.Add("@BillAmount", source.billamount, DbType.Decimal);
                param.Add("@BillHours", source.billhours, DbType.Decimal);
                if (!string.IsNullOrWhiteSpace(source.billnotes))
                    param.Add("@BillNotes", source.billnotes, DbType.String);
                
                param.Add("@CommissionPaidDate", source.commissionpaiddate, DbType.Date);
                param.Add("@CommissionAmount", source.commissionamount, DbType.Decimal);
                param.Add("@CommissionHours", source.commissionhours, DbType.Decimal);
                if (!string.IsNullOrWhiteSpace(source.commissionnotes))
                    param.Add("@CommissionNotes", source.commissionnotes, DbType.String);

                param.Add("@Status", source.status, DbType.String);
                param.Add("@LoginId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SAVEINVOICE, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Invoice saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }
    }
}