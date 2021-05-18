using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace StaffingService.DataAccess
{
    internal class EmailDal
    {
        private static ISqlDbConnection _dbConnection;
        private static EmailDal instance = null;
        private static readonly object padlock = new object();

        public EmailDal()
        {
        }

        public static EmailDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new EmailDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }


        internal async Task<EmailConfigDetails> GetEmailConfigDetails()
        {
            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryAsync<EmailConfigDetails>(Constants.StoredProcedure.GETEMAILCONFIGDETAILS, null, null, null, CommandType.StoredProcedure);
                return data.ToList()[0];
            }
        }

        internal async Task<ResponseModel> GetEmailDetails()
        {
            ResponseModel response = new ResponseModel();
            List<EmailType> types = new List<EmailType>();
            List<ToEmailDetails> toDetails = new List<ToEmailDetails>();
            List<EmailDetails> details = new List<EmailDetails>();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                var data = await conn.QueryMultipleAsync(Constants.StoredProcedure.GETEMAILDETAILS, null, null, null, CommandType.StoredProcedure);
                types = data.Read<EmailType>().ToList();
                toDetails = data.Read<ToEmailDetails>().ToList();

                types.ForEach((type) =>
                {
                    EmailDetails detail = new EmailDetails()
                    {
                        emailtypeid = type.EmailTypeId,
                        name = type.Name,
                        description = type.Description,
                        todetails = new List<Models.ToEmailDetails>()
                    };

                    var toEmails = toDetails.Where(x => x.emailtypeid == type.EmailTypeId).ToList();

                    if (toEmails != null && toEmails.Count > 0)
                        detail.todetails = toEmails;

                    details.Add(detail);
                });

                response = Common.GetResponse(details.Count);
                response.Output = details;
            }

            return response;
        }
    }
}