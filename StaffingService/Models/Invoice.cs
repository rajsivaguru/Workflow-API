using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static StaffingService.Models.Config;

namespace StaffingService.Models
{
    public class Accounts
    {
        public class InvoiceSupportingDetail
        {
            public List<EmploymentType> employmenttypes { get; set; }
            public List<CommunicationMode> communicationmodes { get; set; }
            public List<BillingFrequency> billingfrequencies { get; set; }
            public List<CustomerVendor> customervendors { get; set; }
        }
        
        public class Consultant
        {
            public int consultantid { get; set; }
            public int employmenttypeid { get; set; }
            public string code { get; set; }
            public int billingfrequencyid { get; set; }
            public string frequency { get; set; }
            public int communicationid { get; set; }
            public string mode { get; set; }

            [Required(ErrorMessage ="First Name is required")]
            public string firstname { get; set; }
            public string middlename { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            public string lastname { get; set; }
            public string fullname { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            public string email { get; set; }

            [Required(ErrorMessage = "Phone # is required")]
            public string phone { get; set; }
            public string customer { get; set; }
            public string endclient { get; set; }
            public string billpayto { get; set; }

            //[Required(ErrorMessage = "Invoice Start Date is required")]
            public DateTime invoicestartdate { get; set; }

            //[Required(ErrorMessage = "Start Date is required")]
            public DateTime startdate { get; set; }
            public DateTime? enddate { get; set; }

            //[Required(ErrorMessage = "Bill Rate is required")]
            public decimal billrate { get; set; }

            //[Required(ErrorMessage = "Pay Rate is required")]
            public decimal payrate { get; set; }

            public decimal? commissionrate { get; set; }
            public string commissionto { get; set; }

            public string invoiceemail { get; set; }
            public string portalurl { get; set; }
            public string portaluser { get; set; }
            public string portalpwd { get; set; }
            public string portalnotes { get; set; }
        }

        public class Invoice
        {
            public int invoiceid { get; set; }
            public int timesheetinvoicestatusdetailid { get; set; }
            public int statusid { get; set; }
            public string name { get; set; }
            public string customer { get; set; }
            public string endclient { get; set; }
            public string billpayto { get; set; }
            public string employmenttype { get; set; }
            public string frequency { get; set; }
            public bool isw2 { get; set; }
            public bool hascommission { get; set; }
            public bool shouldsendinvoice { get; set; }
            public DateTime invoicestartdate { get; set; }
            public DateTime invoiceenddate { get; set; }
            public string status { get; set; }
            public int statussortorder { get; set; }
            public decimal tsexpectedhours { get; set; }
            public decimal? tsactualhours { get; set; }            
            public DateTime? tsreceiveddate { get; set; }
            public DateTime? tsverifieddate { get; set; }
            public string tsnotes { get; set; }
            public DateTime? invoicecreateddate { get; set; }
            public string invoicenumber { get; set; }
            public decimal invoiceamount { get; set; }
            public string invoicenotes { get; set; }
            public DateTime? invoicesentdate { get; set; }
            public string invoicesentnotes { get; set; }
            public DateTime? paymentreceiveddate { get; set; }
            public decimal paymentamount { get; set; }
            public bool ispartialpayment { get; set; }
            public string paymentnotes { get; set; }
            public List<PaymentHistory> paymenthistory { get; set; }
            public DateTime? billpaiddate { get; set; }
            public decimal billamount { get; set; }
            public decimal billhours { get; set; }
            public string billnotes { get; set; }
            public DateTime? commissionpaiddate { get; set; }
            public decimal commissionamount { get; set; }
            public decimal commissionhours { get; set; }
            public string commissionnotes { get; set; }
            public decimal completepercent { get; set; }
        }

        public class PaymentHistory
        {
            public DateTime paymentreceiveddate { get; set; }
            public decimal paymentamount { get; set; }
            public bool ispartialpayment { get; set; }
            public string paymentnotes { get; set; }
        }
    }
}