using System.Data;

namespace CupsellCloneAPI.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetDbConnection();
    }
}