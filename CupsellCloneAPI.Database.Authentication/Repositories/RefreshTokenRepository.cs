using CupsellCloneAPI.Database.Authentication.Models;
using CupsellCloneAPI.Database.Factory;
using Dapper;

namespace CupsellCloneAPI.Database.Authentication.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<RefreshTokenDto?> GetByToken(string token)
        {
            using var conn = await _connectionFactory.GetSqlDbConnection();
            const string sql = $@"
SELECT
    Id as {nameof(RefreshTokenDto.Id)},
    RefreshToken as {nameof(RefreshTokenDto.RefreshToken)},
    UserId as {nameof(RefreshTokenDto.UserId)}
FROM [users].[UserRefreshTokens]
WHERE RefreshToken = @RefreshToken";

            return await conn.QueryFirstOrDefaultAsync<RefreshTokenDto>(
                sql: sql,
                param: new { RefreshToken = token }
            );
        }

        public async Task<Guid> Create(Guid userId, string refreshToken)
        {
            using var conn = await _connectionFactory.GetSqlDbConnection();
            var newGuid = Guid.NewGuid();
            const string sql = @"
INSERT INTO [users].[UserRefreshTokens]
(Id, UserId, RefreshToken)
VALUES
(@Id, @UserId, @RefreshToken)";

            await conn.ExecuteAsync(
                sql: sql,
                param: new
                {
                    Id = newGuid,
                    UserId = userId,
                    RefreshToken = refreshToken
                }
            );
            return newGuid;
        }

        public async Task Delete(Guid id)
        {
            using var conn = await _connectionFactory.GetSqlDbConnection();
            const string sql = @"
DELETE FROM [users].[UserRefreshTokens]
WHERE Id = @Id";

            await conn.ExecuteAsync(
                sql: sql,
                param: new { Id = id });
        }

        public async Task DeleteByUserId(Guid userId)
        {
            using var conn = await _connectionFactory.GetSqlDbConnection();
            const string sql = @"
DELETE FROM [users].[UserRefreshTokens]
WHERE UserId = @UserId";

            await conn.ExecuteAsync(
                sql: sql,
                param: new { UserId = userId });
        }
    }
}