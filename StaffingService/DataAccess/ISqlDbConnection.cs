using System.Data.SqlClient;

namespace StaffingService.DataAccess
{
    public interface ISqlDbConnection
    {
        SqlConnection Connection { get; }
    }
}
