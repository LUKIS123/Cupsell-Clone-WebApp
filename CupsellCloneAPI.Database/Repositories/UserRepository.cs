﻿using CupsellCloneAPI.Database.Entities.User;
using CupsellCloneAPI.Database.Factory;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Dapper;

namespace CupsellCloneAPI.Database.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetById(Guid id)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1
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
    U.IsVerified as {nameof(User.IsVerified)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [users].[Users] U
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
WHERE U.Id = @Id";

        var result = await conn.QueryAsync<User, Role, User>(
            sql: sql,
            map: (user, role) =>
            {
                user.Role = role;
                return user;
            },
            param: new { Id = id },
            splitOn: nameof(Role.Id)
        );

        return result.FirstOrDefault();
    }

    public async Task<User?> GetByEmail(string email)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1
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
    U.IsVerified as {nameof(User.IsVerified)},
    R.Id as {nameof(Role.Id)},
    R.Name as {nameof(Role.Name)}
FROM [users].[Users] U
    INNER JOIN [users].[Roles] R
    ON U.RoleId = R.Id
WHERE U.Email = @Email";

        var result = await conn.QueryAsync<User, Role, User>(
            sql: sql,
            map: (user, role) =>
            {
                user.Role = role;
                return user;
            },
            param: new { Email = email },
            splitOn: nameof(Role.Id)
        );

        return result.FirstOrDefault();
    }

    public async Task<Guid> AddNewUser(
        string email, string username, string password, string phoneNumber, string? name,
        string? lastName, DateTime? dateOfBirth, string? address, int roleId)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        var newGuid = Guid.NewGuid();
        const string sql = @"
INSERT INTO [users].[Users]
(Id, Email, Username, PasswordHash, PhoneNumber, Name, LastName, DateOfBirth, Address, RoleId)
VALUES
(@Id, @Email, @Username, @PasswordHash, @PhoneNumber, @Name, @LastName, @DateOfBirth, @Address, @RoleId)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Id = newGuid, Email = email, Username = username, PassWordHash = password, PhoneNumber = phoneNumber,
                Name = name, LastName = lastName, DateOfBirth = dateOfBirth,
                AddRess = address, RoleId = roleId
            }
        );
        return newGuid;
    }

    public async Task AddNewUser(User user)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = @"
INSERT INTO [users].[Users]
(Id, Email, Username, PasswordHash, PhoneNumber, Name, LastName, DateOfBirth, Address, RoleId)
VALUES
(@Id, @Email, @Username, @PasswordHash, @PhoneNumber, @Name, @LastName, @DateOfBirth, @Address, @RoleId)";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                user.Id, user.Email, user.Username, user.PasswordHash, user.PhoneNumber, user.Name, user.LastName,
                user.DateOfBirth, user.Address, user.RoleId
            }
        );
    }

    public async Task UpdateUser(Guid id, string email, string username, string password, string phoneNumber,
        string? name,
        string? lastName, DateTime? dateOfBirth, string? address, int roleId)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [users].[Users]
SET
    Email = @Email,
    Username = @Username,
    PasswordHash = @PasswordHash,
    PhoneNumber = @PhoneNumber,
    Name = @Name,
    LastName = @LastName,
    DateOfBirth = @DateOfBirth,
    Address = @Address,
    RoleId = @RoleId
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                Email = email, Username = username, Password = password, PhoneNumber = phoneNumber, Name = name,
                LastName = lastName, DateOfBirth = dateOfBirth, AddRess = address, RoleId = roleId, Id = id
            }
        );
    }

    public async Task UpdateUserVerificationStatusTrue(Guid id)
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = @"
UPDATE [users].[Users]
SET
    IsVerified = @IsVerified
WHERE Id = @Id";

        await conn.ExecuteAsync(
            sql: sql,
            param: new
            {
                IsVerified = true, Id = id
            }
        );
    }

    public async Task<IEnumerable<Role>> GetUserRoles()
    {
        using var conn = await _connectionFactory.GetSqlDbConnection();
        const string sql = $@"
SELECT TOP 1000
    Id as {nameof(Role.Id)},
    Name as {nameof(Role.Name)}
FROM [users].[Roles]";

        return await conn.QueryAsync<Role>(
            sql: sql
        );
    }
}