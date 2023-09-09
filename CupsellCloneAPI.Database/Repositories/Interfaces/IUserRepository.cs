﻿using CupsellCloneAPI.Database.Entities.User;

namespace CupsellCloneAPI.Database.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetById(Guid id);

    Task<Guid> AddNewUser(string email, string username, string password, string phoneNumber, string? name,
        string? lastName, DateTime? dateOfBirth, string? address, int roleId);

    Task UpdateUser(Guid id, string email, string username, string password, string phoneNumber, string? name,
        string? lastName, DateTime? dateOfBirth, string? address, int roleId);

    Task<IEnumerable<Role>> GetUserRoles();
}