using System.Data;

namespace CupsellCloneAPI.Database.Factory
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetSqlDbConnection();
    }
}