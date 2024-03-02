using System.Data;

namespace CupsellCloneAPI.Database.Factory
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> GetSqlDbConnection();
    }
}