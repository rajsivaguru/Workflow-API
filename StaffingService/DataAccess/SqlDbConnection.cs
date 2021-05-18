using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace StaffingService.DataAccess
{
    internal class SqlDbConnection : ISqlDbConnection
    {
        private string _connectionString = ConfigurationManager.ConnectionStrings["DBConn"].ToString();
        
        public SqlConnection Connection
        {
            get
            {
                var conn = new SqlConnection(_connectionString);

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                return conn;
            }
        }
    }

    internal class SqlDbConnection2
    {
        private static string _connectionString = ConfigurationManager.ConnectionStrings["DBConn"].ToString();

        private static SqlDbConnection2 instance = null;
        private static SqlConnection connection = null;
        private static readonly object padlock = new object();

        private SqlDbConnection2()
        {
        }

        public SqlConnection Connection
        {
            get
            {
                return connection;
            }
        }
        
        public static SqlDbConnection2 Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SqlDbConnection2();
                    }
                    if (connection == null || string.IsNullOrWhiteSpace(connection.ConnectionString))
                    {
                        connection = new SqlConnection(_connectionString);
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                    }
                    return instance;
                }
            }
        }
    }
}