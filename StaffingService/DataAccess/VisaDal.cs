using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StaffingService.DataAccess
{
    public class VisaDal
    {
        private static ISqlDbConnection _dbConnection;
        private static VisaDal instance = null;
        private static readonly object padlock = new object();
        
        private VisaDal()
        {

        }

        public static VisaDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new VisaDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }


        internal async Task<ResponseModel> GetNoteQuestions()
        {
            ResponseModel response = new ResponseModel();
            List<NoteQuestion> result = new List<NoteQuestion>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var results = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETNOTEQUESTIONS, null, null, null, CommandType.StoredProcedure);

                result = results.Read<NoteQuestion>().ToList();
                List<ScreenOption> options = results.Read<ScreenOption>().ToList();

                result.ForEach((q) =>
                {
                    if(q.questiontype == "Rating" && q.MaxSetRating > 0)
                    {
                        q.maxrate = new List<int>();
                        for (int i = 1; i <= q.MaxSetRating; i++)
                            q.maxrate.Add(i);
                    }
                    q.options = new List<ScreenOption>();
                    List<ScreenOption> qOptions = options.FindAll(x => x.questionid == q.questionid);

                    q.options.AddRange(qOptions);
                });

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetScreeQuestions()
        {
            ResponseModel response = new ResponseModel();
            List<ScreenQuestion> result = new List<ScreenQuestion>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var results = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETSCREENQUESTIONS, null, null, null, CommandType.StoredProcedure);
                
                result = results.Read<ScreenQuestion>().ToList();
                List<ScreenOption> options = results.Read<ScreenOption>().ToList();

                result.ForEach((q) =>
                {
                    q.options = new List<ScreenOption>();
                    List<ScreenOption> qOptions = options.FindAll(x => x.questionid == q.questionid);
                    
                    q.options.AddRange(qOptions);
                });

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveScreenQuestion(ScreenQuestion source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                List<XElement> toAdd = new List<XElement>();
                DynamicParameters param = new DynamicParameters();

                if (source.questionid > 0)
                    param.Add("@Id", source.questionid, DbType.Int32);
                if (source.parentquestionid.HasValue && source.parentquestionid > 0)
                    param.Add("@ParentQuestionId", source.parentquestionid, DbType.Int32);
                if (source.hint.Trim().Length > 0)
                    param.Add("@Hint", source.hint, DbType.String);

                param.Add("@QuestionNumber", source.questionnumber.Trim(), DbType.String);
                param.Add("@Question", source.question.Trim(), DbType.String);
                param.Add("@QuestionType", source.questiontype.Trim(), DbType.String);
                param.Add("@ShowNA", source.showNA, DbType.Boolean);
                param.Add("@IsMandatory", source.ismandatory, DbType.Boolean);
                param.Add("@IsParent", source.isparent, DbType.Boolean);
                param.Add("@LoginUserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGESCREENQUESTION, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                if (source.options != null && source.options.Count > 0  && result > 0)
                {
                    source.options.ForEach((x) =>
                    {
                        toAdd.Add(new XElement("Option", new XAttribute("SequenceOrder", toAdd.Count + 1)
                                                , new XAttribute("ScreenQuestionId", result)
                                                , new XAttribute("Option", x.option ?? string.Empty)
                                                , new XAttribute("OptionAction", x.optionaction == null || x.optionaction == "0" ? string.Empty : x.optionaction)
                                                , new XAttribute("IsNA", x.isNA)
                                                , new XAttribute("LoginId", loginId)));
                    });

                    if (toAdd.Count() > 0)
                    {
                        var add = new XElement("Options", toAdd);
                        
                        DynamicParameters param2 = new DynamicParameters();
                        param2.Add("@XMLOptions", add.ToString(), DbType.String);
                        param2.Add("@QuestionId", result, DbType.String);
                        
                        var data2 = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGESCREENOPTION, param2, null, null, CommandType.StoredProcedure);
                        int result2 = 0;

                        if (data2.ToList().Count > 0)
                            result2 = data2.ToList()[0];

                        if (result2 <= 0)
                        {
                            response.ResultStatus = result2;
                            response.SuccessMessage = result2 <= 0 ? string.Empty : "Screening Question saved successfully.";
                            response.ErrorMessage = result2 <= 0 ? "Error occurred while saving screening question option.  Please try again." : string.Empty;
                            return response;
                        }
                    }
                }

                response.ResultStatus = result >= 1 ? 1 : result;
                response.SuccessMessage = result <= 0 ? string.Empty : "Screening Question saved successfully.";
                response.ErrorMessage = result <= 0 ? "Error occurred while saving screening question.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveScreening(ScreenedProcess source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {   
                if (source.questions != null && source.questions.Count > 0)
                {
                    List<XElement> toAdd = new List<XElement>();
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@JobId", source.jobid, DbType.Int32);
                    param.Add("@FirstName", source.firstname.Trim(), DbType.String);
                    param.Add("@LastName", source.lastname.Trim(), DbType.String);
                    param.Add("@CurrentLocation", source.currentlocation.Trim(), DbType.String);
                    param.Add("@Email", source.email.Trim(), DbType.String);
                    param.Add("@Phone", source.phone.Trim(), DbType.String);
                    param.Add("@StartTime", source.starttime, DbType.DateTime);
                    param.Add("@EndTime", source.endtime, DbType.DateTime);

                    if (source.isterminated)
                    {
                        param.Add("@EndReason", source.endreason.Trim(), DbType.String);
                        param.Add("@IsTerminated", source.isterminated, DbType.Boolean);
                    }
                    
                    param.Add("@LoginUserId", loginId, DbType.Int32);
                                        
                    source.questions.ForEach((x) =>
                    {
                        toAdd.Add(new XElement("Question", new XAttribute("SequenceOrder", toAdd.Count + 1)
                                                , new XAttribute("ScreenQuestionId", x.questionid)
                                                , new XAttribute("Response", x.option ?? string.Empty)
                                                , new XAttribute("IsNA", x.optionna)));
                    });

                    if (toAdd.Count() > 0)
                    {
                        var add = new XElement("Questions", toAdd);

                        param.Add("@XMLQuestions", add.ToString(), DbType.String);
                        
                        var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGESCREENPROCESS, param, null, null, CommandType.StoredProcedure);
                        int result = 0;

                        if (data.ToList().Count > 0)
                            result = data.ToList()[0];

                        response.ResultStatus = result;
                        response.SuccessMessage = result <= 0 ? string.Empty : "Screening Process Question(s) saved successfully.";
                        response.ErrorMessage = result <= 0 ? "Error occurred while saving screening process question(s).  Please try again." : string.Empty;
                        return response;
                    }
                }
            }

            return response;
        }

        internal async Task<ResponseModel> DeleteQuestion(int questionId, int loginId)
        {
            ResponseModel response = new ResponseModel()
            {
                ErrorMessage = "Question not deleted, please try again.",
                RequestType = Constants.RequestType.POST,
                ResultStatus = -1
            };

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@Id", questionId, DbType.Int32);
                param.Add("IsDeletable", true, DbType.Boolean);
                param.Add("@LoginUserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEQUESTION, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Visa Question deleted successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while deleting visa question.  Please try again." : string.Empty;
            }

            return response;
        }


        #region Unused Old code

        internal async Task<ResponseModel> GetCategories()
        {
            ResponseModel response = new ResponseModel();
            List<Category> result = new List<Category>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                dynamic data = await conn.QueryAsync<Category>(Constants.StoredProcedure.GETCAGETORIES, null, null, null, CommandType.StoredProcedure);
                result = (List<Category>)data;

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetQuestions()
        {
            ResponseModel response = new ResponseModel();
            List<Question> result = new List<Question>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                dynamic data = await conn.QueryAsync<Question>(Constants.StoredProcedure.GETQUESTIONS, null, null, null, CommandType.StoredProcedure);
                result = (List<Question>)data;

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> GetExternalConsultants()
        {
            ResponseModel response = new ResponseModel();
            List<ExternalConsultant> result = new List<ExternalConsultant>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                dynamic data = await conn.QueryAsync<ExternalConsultant>(Constants.StoredProcedure.GETEXTERNALCONSULTANTS, null, null, null, CommandType.StoredProcedure);
                result = (List<ExternalConsultant>)data;

                response = Common.GetResponse(result.Count);
                response.Output = result;
            }

            return response;
        }

        internal async Task<ResponseModel> SaveQuestion(Question source, int loginId)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                if (source.questionid > 0)
                    param.Add("@Id", source.questionid, DbType.Int32);

                param.Add("@CategoryId", source.categoryid, DbType.Int32);
                param.Add("@Question", source.question.Trim(), DbType.String);
                param.Add("@IsMandatory", source.ismandatory, DbType.Boolean);
                param.Add("@LoginUserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEQUESTION, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Visa Question saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving visa question.  Please try again." : string.Empty;

            }

            return response;
        }

        internal async Task<ResponseModel> SaveExternalConsultant(ExternalConsultant source, int loginId)
        {
            ResponseModel response = new ResponseModel()
            {
                ErrorMessage = "Question not saved, please try again.",
                RequestType = Constants.RequestType.POST,
                ResultStatus = -1
            };

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                if (source.consultantid > 0)
                    param.Add("@Id", source.consultantid, DbType.Int32);

                param.Add("@Email", source.email.Trim(), DbType.String);
                param.Add("@ContactNumber", source.contactnumber.Trim(), DbType.String);
                param.Add("@WorkStatus", source.workauthorization.Trim(), DbType.String);
                param.Add("@TotalExperience", source.totalexperience.Trim(), DbType.String);
                param.Add("@RelevantExperience", source.relevantexperience.Trim(), DbType.String);
                param.Add("@Technology", source.technology.Trim(), DbType.String);
                param.Add("@location", source.currentlocation.Trim(), DbType.String);
                param.Add("@CanRelocate", source.canrelocate, DbType.Boolean);
                param.Add("@InProject", source.inproject, DbType.Boolean);
                param.Add("@NoticePeriod", source.noticeperiod.Trim(), DbType.String);
                param.Add("@Education", source.education.Trim(), DbType.String);
                param.Add("@Certification", source.certification.Trim(), DbType.String);
                param.Add("@IdType", source.identificationtype.Trim(), DbType.String);
                param.Add("@IdNumber", source.identificationid.Trim(), DbType.String);
                param.Add("@AdditionalInfo", source.additionalinfo.Trim(), DbType.String);

                if (source.responses != null && source.responses.Count > 0)
                {
                    ////Todo:  need to add params.
                }

                param.Add("@LoginUserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEEXTERNALCONSULTANTS, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "External Consultant information saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving external consultant information.  Please try again." : string.Empty;
            }

            return response;
        }

        internal async Task<ResponseModel> DeleteExternalConsultant(int consultantId, int loginId)
        {
            ResponseModel response = new ResponseModel()
            {
                ErrorMessage = "Question not deleted, please try again.",
                RequestType = Constants.RequestType.POST,
                ResultStatus = -1
            };

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@Id", consultantId, DbType.Int32);
                param.Add("IsDeletable", true, DbType.Boolean);
                param.Add("@LoginUserId", loginId, DbType.Int32);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.MANAGEEXTERNALCONSULTANTS, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "External Consultant information deleted successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while deleting External Consultant information.  Please try again." : string.Empty;
            }

            return response;
        }

        #endregion
    }
}