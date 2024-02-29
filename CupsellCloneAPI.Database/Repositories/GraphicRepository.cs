using CupsellCloneAPI.Database.Entities.Product;
using CupsellCloneAPI.Database.Entities.User;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;

namespace CupsellCloneAPI.Database.Repositories;

public class GraphicRepository : IGraphicRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public GraphicRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Graphic?> GetById(Guid id)
    {
        const string sql = $@"
SELECT TOP 1
    G.Id as {nameof(Graphic.Id)},
    G.Name as {nameof(Graphic.Name)},
    G.SellerId as {nameof(Graphic.SellerId)},
    G.Description as {nameof(Graphic.Description)},
    U.Id as {nameof(User.Id)},
    U.Email as {nameof(User.Email)},
    U.Username as {nameof(User.Username)},
    U.PasswordHash as {nameof(User.PasswordHash)},
    U.PhoneNumber as {nameof(User.PhoneNumber)},
    U.Name as {nameof(User.Name)},
    U.LastName as {nameof(User.LastName)},
    U.DateOfBirth as {nameof(User.DateOfBirth)},
    U.Address as {nameof(User.Address)},
    U.RoleId as {nameof(User.RoleId)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [products].[Graphics] G
    INNER JOIN [users].[Users] U
    ON G.SellerId = U.Id
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
WHERE G.Id = @Id";

        var result = await QueryGraphicsAsync(sql, new { Id = id });
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<Graphic>> GetByUserId(Guid sellerId)
    {
        const string sql = $@"
SELECT
    G.Id as {nameof(Graphic.Id)},
    G.Name as {nameof(Graphic.Name)},
    G.SellerId as {nameof(Graphic.SellerId)},
    G.Description as {nameof(Graphic.Description)},
    U.Id as {nameof(User.Id)},
    U.Email as {nameof(User.Email)},
    U.Username as {nameof(User.Username)},
    U.PasswordHash as {nameof(User.PasswordHash)},
    U.PhoneNumber as {nameof(User.PhoneNumber)},
    U.Name as {nameof(User.Name)},
    U.LastName as {nameof(User.LastName)},
    U.DateOfBirth as {nameof(User.DateOfBirth)},
    U.Address as {nameof(User.Address)},
    U.RoleId as {nameof(User.RoleId)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [products].[Graphics] G
    INNER JOIN [users].[Users] U
    ON G.SellerId = U.Id
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
WHERE G.SellerId = @SellerId";

        return await QueryGraphicsAsync(sql, new { SellerId = sellerId });
    }

    public async Task<Guid> Create(string name, Guid sellerId, string? description)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [products].[Graphics]
(Id, Name, SellerId, Description)
VALUES
(@Id, @Name, @SellerId, @Description)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = newGuid, Name = name, SellerId = sellerId, Description = description }
        );

        return newGuid;
    }

    public async Task Delete(Guid id)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
DELETE FROM [products].[Graphics]
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id });
    }

    public async Task Update(Guid id, string newName, string? description)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [products].[Graphics]
SET
    Name = @Name,
    Description = @Description
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new { Id = id, Name = newName, Description = description }
        );
    }

    private async Task<IEnumerable<Graphic>> QueryGraphicsAsync(string sql, object param)
    {
        using var conn = _connectionFactory.GetSqlDbConnection();
        return await conn.QueryAsync<Graphic, User, Role, Graphic>(
            sql: sql,
            map: (graphic, user, userRole) =>
            {
                user.Role = userRole;
                graphic.Seller = user;
                return graphic;
            },
            param: param,
            splitOn: nameof(User.Id)
        );
    }
}