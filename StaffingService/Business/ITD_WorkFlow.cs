using System.Threading.Tasks;

namespace WorkFlow.Business
{
    public interface ITD_WorkFlow
    {
        string BindUsers(string status, string loginid);
        
        string BindPriority();
        string BindRoles(string sSearch);
        string SaveUser(string sUserModel, string loginid);
        Task<string> SynchJobsXML();
        string BindJobs(string loginid);
        string BindJobStatus(string jobassignmentid);

        string UpdateJobStatus(string jobassignmentid, string statusid, string comment, string userid);
        
        string SearchJob(string keyword);

        string SearchUser(string keyword);

        string BindJobStatusHistory(string jobassignmentid);

        string SaveJobUser(string userid, string jobid, string priorityid, string clientname, string loginid);
    }
}
    




