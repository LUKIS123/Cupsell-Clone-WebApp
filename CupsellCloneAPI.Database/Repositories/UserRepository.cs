namespace CupsellCloneAPI.Database.Repositories
{
    public class UserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        // 1;10;00
    }
}