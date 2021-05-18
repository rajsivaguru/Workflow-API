using Dapper;
using StaffingService.Models;
using StaffingService.Util;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StaffingService.DataAccess
{
    internal class ProfileSearchDal
    {
        private static ISqlDbConnection _dbConnection;
        private static ProfileSearchDal instance = null;
        private static readonly object padlock = new object();

        public ProfileSearchDal()
        {
        }

        public static ProfileSearchDal Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ProfileSearchDal();
                    }
                    if (_dbConnection == null)
                    {
                        _dbConnection = new SqlDbConnection();
                    }

                    return instance;
                }
            }
        }

        internal async Task<ResponseModel> SaveProfileSearch(ProfileSearchCriteria source)
        {
            ResponseModel response = new ResponseModel();

            using (IDbConnection conn = _dbConnection.Connection)
            {
                DynamicParameters param = new DynamicParameters();

                param.Add("@userid", source.userid, DbType.Int32);

                if(source.jobid.HasValue && source.jobid > 0)
                    param.Add("@jobid", source.jobid, DbType.Int32);
                if(!string.IsNullOrWhiteSpace(source.title))
                    param.Add("@title", source.title, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.location))
                    param.Add("@location", source.location, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.headline))
                    param.Add("@headline", source.headline, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.skill1))
                    param.Add("@skill1", source.skill1, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.skill2))
                    param.Add("@skill2", source.skill2, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.skill3))
                    param.Add("@skill3", source.skill3, DbType.String);
                if (!string.IsNullOrWhiteSpace(source.searchstring))
                    param.Add("@searchstring", source.searchstring, DbType.String);

                param.Add("@searchengine", source.searchengine, DbType.String);
                param.Add("@isjobseeker", source.isjobseeker, DbType.Boolean);
                param.Add("@isoverride", source.isoverride, DbType.Boolean);

                var data = await conn.QueryAsync<int>(Constants.StoredProcedure.SAVEPROFILESEARCH, param, null, null, CommandType.StoredProcedure);
                int result = 0;

                if (data.ToList().Count > 0)
                    result = data.ToList()[0];

                response.ResultStatus = result;
                response.RequestType = Constants.RequestType.POST;
                response.SuccessMessage = result == 0 ? string.Empty : "Saved successfully.";
                response.ErrorMessage = result == 0 ? "Error occurred while saving.  Please try again." : string.Empty;
            }

            return response;
        }

    }
}