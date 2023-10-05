using CupsellCloneAPI.Database.Authentication.Models;
using CupsellCloneAPI.Database.Factory;
using Dapper;

namespace CupsellCloneAPI.Database.Authentication.Repositories;

public class VerificationTokenRepository : IVerificationTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public VerificationTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<VerificationTokenDto> GetByToken(string verificationToken)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT
    UserId as {nameof(VerificationTokenDto.UserId)},
    VerificationToken as {nameof(VerificationTokenDto.VerificationToken)}
FROM [users].[UserVerificationTokens]
WHERE VerificationToken = @VerificationToken";

        return await conn.QuerySingleOrDefaultAsync<VerificationTokenDto>(
            sql: sql,
            param: new { VerificationToken = verificationToken }
        );
    }

    public async Task<VerificationTokenDto> GetByUserId(Guid userId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT
    UserId as {nameof(VerificationTokenDto.UserId)},
    VerificationToken as {nameof(VerificationTokenDto.VerificationToken)}
FROM [users].[UserVerificationTokens]
WHERE UserId = @UserId";

        return await conn.QuerySingleOrDefaultAsync<VerificationTokenDto>(
            sql: sql,
            param: new { UserId = userId }
        );
    }

    public async Task Create(Guid userId, string verificationToken)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
INSERT INTO [users].[UserVerificationTokens]
(UserId, VerificationToken)
VALUES
(@UserId, @VerificationToken)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                UserId = userId,
                VerificationToken = verificationToken
            }
        );
    }

    public async Task DeleteByUserId(Guid userId)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [users].[UserVerificationTokens]
WHERE UserId = @UserId";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { UserId = userId });
    }
}