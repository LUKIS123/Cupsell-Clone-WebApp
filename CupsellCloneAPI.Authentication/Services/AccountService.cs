using System.Security.Claims;
using CupsellCloneAPI.Authentication.Authenticators;
using CupsellCloneAPI.Authentication.Exceptions;
using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.Settings;
using CupsellCloneAPI.Authentication.TokenGenerators;
using CupsellCloneAPI.Authentication.TokenValidators;
using CupsellCloneAPI.Database.Authentication.Repositories;
using CupsellCloneAPI.Database.Entities.User;
using CupsellCloneAPI.Database.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace CupsellCloneAPI.Authentication.Services;

public class AccountService : IAccountService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly IAuthenticator _authenticator;

    public AccountService(
        IPasswordHasher<User> passwordHasher,
        AuthenticationSettings authenticationSettings,
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenValidator refreshTokenValidator,
        IAuthenticator authenticator)
    {
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenValidator = refreshTokenValidator;
        _authenticator = authenticator;
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
            Role = default!
        };
        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
        newUser.PasswordHash = hashedPassword;
        await _userRepository.AddNewUser(newUser);
    }

    public async Task<string> GenerateJwt(LoginUserDto dto)
    {
        var user = await _userRepository.GetByEmail(dto.Email);

        if (user is null)
        {
            throw new InvalidLoginParamsException("Invalid username or password...");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidLoginParamsException("Invalid username or password...");
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
            new(ClaimTypes.Role, user.Role.Name),
            new(ClaimTypes.Email, user.Email),
            new("Username", user.Username)
        };
        if (user.DateOfBirth.HasValue)
        {
            claims.Add(new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")));
        }

        return _tokenGenerator.GenerateJwt(_authenticationSettings.JwtKey, _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtAudience, DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes),
            claims);
    }

    public string GenerateRefreshTokenAndSave(Guid userId)
    {
        return _tokenGenerator.GenerateJwt(_authenticationSettings.RefreshTokenKey,
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtAudience,
            DateTime.UtcNow.AddMinutes(_authenticationSettings.RefreshTokenExpireMinutes));
    }

    public async Task<AuthenticatedUserResponseDto> GenerateRefreshJwt(string refreshToken)
    {
        var isValidToken = _refreshTokenValidator.Validate(refreshToken);
        if (!isValidToken)
        {
            throw new InvalidLoginParamsException("Could not use refresh token...");
        }

        var refreshTokenDto = await _refreshTokenRepository.GetByToken(refreshToken);
        if (refreshTokenDto is null)
        {
            throw new InvalidLoginParamsException("Could not use refresh token...");
        }

        var user = await _userRepository.GetById(refreshTokenDto.UserId);
        if (user is null)
        {
            throw new Exception();

            // TODO!!!
        }

        var response = await _authenticator.Authenticate(user);
        return response;
    }

    /// <summary>
    /// /////////////////////
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    /// 
    public AccessTokenDto GenerateToken(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
        };

        var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationSettings.JwtExpireMinutes);
        var value = _tokenGenerator.GenerateJwt(
            _authenticationSettings.JwtKey,
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtAudience,
            expirationTime,
            claims
        );

        return new AccessTokenDto()
        {
            Value = value,
            ExpirationTime = expirationTime
        };
    }

    public string GenerateToken()
    {
        var expirationTime = DateTime.UtcNow.AddMinutes(_authenticationSettings.RefreshTokenExpireMinutes);

        return _tokenGenerator.GenerateJwt(
            _authenticationSettings.RefreshTokenKey,
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtAudience,
            expirationTime);
    }
}