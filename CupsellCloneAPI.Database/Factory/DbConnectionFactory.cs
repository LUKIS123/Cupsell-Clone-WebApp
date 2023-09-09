using System.Data;
using Microsoft.Data.SqlClient;

namespace CupsellCloneAPI.Database.Factory
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection GetSqlDbConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}