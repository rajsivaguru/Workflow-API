namespace StaffingService.Util
{
    internal class Constants
    {
        internal class ResponseResult
        {
            internal static int AUTHORIZATIONNOTIMPLEMENTED = -4;
            internal static int UNAUTHORIZED = -3;
            internal static int MISSINGDATA = -2;
            internal static int ERROR = -1;
            internal static int NODATA = 0;
            internal static int SUCCESS = 1;
        }

        internal static class ErrorMessage
        {
            internal const string MISSINGDATA = "One or more required field(s) are missing or the value is not valid.";
            internal const string UNAUTHORIZED = "You are not authorized to access this content.  Please re-LOGIN and try again or contact your administrator.";
            internal const string AUTHORIZATIONNOTIMPLEMENTED = "Authorization Not Iimplemented - You are not authorized to access this content.  Please contact your administrator.";
        }

        internal static class RequestType
        {
            internal const string GET = "GET";
            internal const string POST = "POST";
        }

        internal static class ObjectType
        {
            internal const string HTTPREQUESTHEADERS = "HttpRequestHeaders";
        }

        internal class StoredProcedure
        {
            /* Common. */
            internal static string GETSTATUS = "spGetStatus";
            internal static string GETCUSTOMERTYPES = "spGetCustomerTypes";
            internal static string GETCUSTOMERS = "spGetCustomersVendors";
            internal static string MANAGECUSTOMERVENDOR = "spManageCustomerVendor";
            internal static string GETMYNOTIFICATIONS = "spGetMyNotifications";

            /* Related to USER. */
            internal static string GETUSER = "spGetUserDetail";
            internal static string GETALLUSERS = "spGetAllUsers";
            internal static string GETASSIGNMENTUSERS = "spGetAssignmentUsers";
            internal static string GETABSENTUSERS = "spGetAbsentUsers";
            internal static string SAVEABSENTUSERS = "spSaveAbsentUsers";
            internal static string SYNCUSERAFTERLOGIN = "spSyncUserDetails";

            /* Related to REPORT */
            internal static string GETPERIOD = "spGetPeriods";
            internal static string GETUSERSFORREPORT = "spGetUsersForReport";
            internal static string GETJOBREPORT = "spGetJobReport";
            internal static string GETUSERREPORT = "spGetUserReport";
            internal static string GETCLIENTREPORT = "spGetClientReport";
            internal static string GETPROFILESEACHRREPORT = "spGetProfileSearchReport";
            internal static string GETPUNCHREPORT = "spGetPunchReport";

            /* Related to JOBS */
            internal static string GETJOBDETAILS = "spGetJobDetails";
            internal static string GETJOBS = "spGetJobList";
            internal static string GETJOBS_DD = "spGetJobListForDD";
            internal static string SYNCJOB = "spLoadXMLJobs";
            internal static string ASSIGNJOB = "spAssignJobToUsers";

            /* Related to PRIORITY JOBS */
            internal static string GETPRIORITYJOBS = "spGetPriorityJobs";
            internal static string ASSIGNPRIORITYJOB = "spAssignPriorityJob";

            /* Related to PRIORITY JOBS */
            internal static string SaveInterestedJob = "spSaveNotification";
            
            /* Related to CLIENT */
            internal static string GETCLIENT = "spGetClients";
            internal static string MANAGECLIENT = "spManageClient";

            /* Related to Recruiter */
            internal static string GETMYJOBLIST = "spGetMyAssignedJobs";
            internal static string MANAGEMYJOB = "spCUJobAssignmentStatus";

            /* Related to VISA */
            internal static string GETCAGETORIES = "spGetCategories";
            internal static string GETQUESTIONS = "spGetQuestions";
            internal static string MANAGEQUESTION = "spManageQuestion";

            internal static string GETEXTERNALCONSULTANTS = "spGetExternalConsultants";
            internal static string MANAGEEXTERNALCONSULTANTS = "spManageExternalConsultants";

            internal static string GETSCREENQUESTIONS = "spGetScreenQuestions";
            internal static string MANAGESCREENQUESTION = "spManageScreenQuestion";
            internal static string MANAGESCREENOPTION = "spManageScreenOption";
            internal static string MANAGESCREENPROCESS = "spManageScreenProcess";

            /* Related to NOTES */
            internal static string GETNOTEQUESTIONS = "spGetNoteQuestions";

            /* Related to PROFILE SEARCH */
            internal static string SAVEPROFILESEARCH = "spSaveProfileSearch";

            /* Related to PROFILE SEARCH */
            internal static string GETEMAILDETAILS = "spGetEmailDetails";
            internal static string GETEMAILCONFIGDETAILS = "spGetEmailConfigDetails";

            /* Related to INVOICE */
            internal static string GETINVOICESUPPORTDETAILS = "spGetInvoiceSupportDetails";
            internal static string GETCONSULTANTS = "spGetConsultants";
            internal static string SAVECONSULTANT = "spAddConsultant";
            internal static string UPDATECONSULTANT = "spUpdateConsultant";
            internal static string GETACTIVEINVOICES = "spGetActiveInvoices";
            internal static string GETINVOICEDETAILS = "spGetInvoiceDetails";
            internal static string SAVEINVOICE = "spUpdateInvoice";

            /* Related to ATTENDANCE */
            internal static string GETMYPUNCHDETAILS = "spGetMyPunchDetails";
            internal static string SAVEMYPUNCHDETAILS = "spSaveMyPunchDetails";

            /* Related to ErrorLog */
            internal static string CREATELOG = "spInsertLog";
        }
    }
}