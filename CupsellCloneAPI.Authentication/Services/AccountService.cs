using CupsellCloneAPI.Authentication.Authenticators;
using CupsellCloneAPI.Authentication.Exceptions;
using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.TokenValidators;
using CupsellCloneAPI.Database.Authentication.Repositories;
using CupsellCloneAPI.Database.Entities.User;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CupsellCloneAPI.Authentication.Services;

public class AccountService : IAccountService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly IAuthenticator _authenticator;
    private readonly IUserContextService _userContextService;

    public AccountService(IPasswordHasher<User> passwordHasher,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenValidator refreshTokenValidator,
        IAuthenticator authenticator,
        IUserContextService userContextService)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenValidator = refreshTokenValidator;
        _authenticator = authenticator;
        _userContextService = userContextService;
    }

    public async Task RegisterUser(RegisterUserDto dto)
    {
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Username = dto.Username,
            PasswordHash = string.Empty,
            PhoneNumber = dto.PhoneNumber,
            Name = dto.Name,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            RoleId = dto.RoleId,
            IsVerified = false,
            Role = default!
        };
        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.PasswordHash = hashedPassword;

        await _userRepository.AddNewUser(newUser);
    }

    public async Task<AuthenticatedUserResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _userRepository.GetByEmail(loginUserDto.Email);
        if (user is null)
        {
            throw new InvalidLoginParamsException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidLoginParamsException("Invalid username or password");
        }

        return await _authenticator.Authenticate(user);
    }

    public async Task<AuthenticatedUserResponseDto> RefreshUserJwt(string refreshToken)
    {
        var isValidToken = _refreshTokenValidator.Validate(refreshToken);
        if (!isValidToken)
        {
            throw new InvalidLoginParamsException("Could not use refresh token");
        }

        var refreshTokenDto = await _refreshTokenRepository.GetByToken(refreshToken);
        if (refreshTokenDto is null)
        {
            throw new InvalidLoginParamsException("Could not use refresh token");
        }

        await _refreshTokenRepository.Delete(refreshTokenDto.Id);

        var user = await _userRepository.GetById(refreshTokenDto.UserId);
        if (user is null)
        {
            throw new AuthenticationErrorException("Authentication error");
        }

        var response = await _authenticator.Authenticate(user);
        return response;
    }

    public async Task DeleteUserRefreshTokens()
    {
        var userId = _userContextService.GetUserId;
        if (!userId.HasValue)
        {
            throw new ForbidException("You need to be logged in to perform this action");
        }

        await _refreshTokenRepository.DeleteByUserId(userId.Value);
    }


    // public async Task<string> GenerateJwt(LoginUserDto dto)
    // {
    //     var user = await _userRepository.GetByEmail(dto.Email);
    //
    //     if (user is null)
    //     {
    //         throw new InvalidLoginParamsException("Invalid username or password...");
    //     }
    //
    //     var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
    //     if (result == PasswordVerificationResult.Failed)
    //     {
    //         throw new InvalidLoginParamsException("Invalid username or password...");
    //     }
    //
    //     var claims = new List<Claim>()
    //     {
    //         new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //         new(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
    //         new(ClaimTypes.Role, user.Role.Name),
    //         new(ClaimTypes.Email, user.Email),
    //         new("Username", user.Username)
    //     };
    //     if (user.DateOfBirth.HasValue)
    //     {
    //         claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
    //     }
    //
    //     return _tokenGenerator.GenerateJwt(_authenticationSettings.JwtKey, _authenticationSettings.JwtIssuer,
    //         _authenticationSettings.JwtAudience, DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes),
    //         claims);
    // }

    // public AccessTokenDto GenerateToken(User user)
    // {
    //     var claims = new List<Claim>()
    //     {
    //         new Claim("id", user.Id.ToString()),
    //         new Claim(ClaimTypes.Email, user.Email),
    //         new Claim(ClaimTypes.Name, user.Username),
    //     };
    //
    //     var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes);
    //     var value = _tokenGenerator.GenerateJwt(
    //         _authenticationSettings.JwtKey,
    //         _authenticationSettings.JwtIssuer,
    //         _authenticationSettings.JwtAudience,
    //         expirationTime,
    //         claims
    //     );
    //
    //     return new AccessTokenDto()
    //     {
    //         Value = value,
    //         ExpirationTime = expirationTime
    //     };
    // }
}