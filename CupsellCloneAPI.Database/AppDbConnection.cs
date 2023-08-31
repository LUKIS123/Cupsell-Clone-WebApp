using System.Data;
using Microsoft.Data.SqlClient;

namespace CupsellCloneAPI.Database
{
    public class AppDbConnection : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public AppDbConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetDbConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}