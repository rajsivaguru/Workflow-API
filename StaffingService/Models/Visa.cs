using System;
using System.Collections.Generic;

namespace StaffingService.Models
{
    public class Category
    {
        public int categoryid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }

    public class Question
    {
        public int questionid { get; set; }
        public int categoryid { get; set; }
        public string question { get; set; }
        public string category { get; set; }
        public bool ismandatory { get; set; }
        public int loginid { get; set; }
    }

    public class QuestionReponse
    {
        public int questionid { get; set; }
        public string answer { get; set; }
    }

    public class ExternalConsultant
    {
        public int consultantid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string contactnumber { get; set; }
        public string workauthorization { get; set; }
        public string totalexperience { get; set; }
        public string relevantexperience { get; set; }
        public string technology { get; set; }
        public string currentlocation { get; set; }
        public bool? canrelocate { get; set; }
        public bool inproject { get; set; }
        public string noticeperiod { get; set; }
        public string education { get; set; }
        public string certification { get; set; }
        public string identificationtype { get; set; }
        public string identificationid { get; set; }
        public string createdby { get; set; }
        public string createdon { get; set; }
        public string updatedby { get; set; }
        public string updatedon { get; set; }
        public string additionalinfo { get; set; }

        public List<QuestionReponse> responses { get; set; }
    }


    public class NoteQuestion
    {
        public int questionid { get; set; }
        public string question { get; set; }
        public string questiontype { get; set; }
        public string response { get; set; }
        public List<int> maxrate { get; set; }
        public List<ScreenOption> options { get; set; }

        public int MaxSetRating { get; set; }
    }

    public class ScreenQuestion
    {
        public int questionid { get; set; }
        public int? parentquestionid { get; set; }
        public string questionnumber { get; set; }
        public string question { get; set; }
        public string questiontype { get; set; }
        public string hint { get; set; }
        public bool showNA { get; set; }
        public bool ismandatory { get; set; }
        public bool isparent { get; set; }
        public int sortorder { get; set; }
        public int loginid { get; set; }
        public string option { get; set; }
        public string optionna { get; set; }
        public List<ScreenOption> options { get; set; }
    }

    public class ScreenOption
    {
        public int optionid { get; set; }
        public int questionid { get; set; }
        public string question { get; set; }
        public string questionnumber { get; set; }
        public string option { get; set; }
        public string optionaction { get; set; }
        public bool isNA { get; set; }
        public int sortorder { get; set; }
    }

    public class ScreenedProcess
    {
        public int screenprocessid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string currentlocation { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public int jobid { get; set; }
        public string endreason { get; set; }
        public bool isterminated { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int loginid { get; set; }
        public List<ScreenQuestion> questions { get; set; }
    }
}