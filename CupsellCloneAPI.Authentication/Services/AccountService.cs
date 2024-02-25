using CupsellCloneAPI.Authentication.Authenticators;
using CupsellCloneAPI.Authentication.EmailAuthenticationHelper;
using CupsellCloneAPI.Authentication.Exceptions;
using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.TokenValidators;
using CupsellCloneAPI.Core.Utils.Accessors;
using CupsellCloneAPI.Core.Utils.Encryption;
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
    private readonly IVerificationTokenRepository _verificationTokenRepository;
    private readonly IAuthenticator _authenticator;
    private readonly IEncryptionHelper _encryptionHelper;
    private readonly IUserAccessor _userAccessor;
    private readonly IUrlAccessor _urlAccessor;
    private readonly IEmailCommunicationHelper _emailAuthenticationHelper;

    public AccountService(IPasswordHasher<User> passwordHasher,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenValidator refreshTokenValidator,
        IAuthenticator authenticator,
        IVerificationTokenRepository verificationTokenRepository,
        IEncryptionHelper encryptionHelper,
        IUserAccessor userAccessor,
        IUrlAccessor urlAccessor,
        IEmailCommunicationHelper emailAuthenticationHelper)
    {
        _passwordHasher = passwordHasher;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenValidator = refreshTokenValidator;
        _authenticator = authenticator;
        _verificationTokenRepository = verificationTokenRepository;
        _encryptionHelper = encryptionHelper;
        _userAccessor = userAccessor;
        _urlAccessor = urlAccessor;
        _emailAuthenticationHelper = emailAuthenticationHelper;
    }

    public async Task RegisterUser(RegisterUserDto dto)
    {
        var newUserId = Guid.NewGuid();
        var newUser = new User
        {
            Id = newUserId,
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

        var verificationToken = Guid.NewGuid().ToString();
        await _verificationTokenRepository.Create(newUserId, verificationToken);

        var hashedToken = _encryptionHelper.Encrypt(verificationToken);
        var result = await _emailAuthenticationHelper.SendVerificationEmail(
            hashedToken,
            _urlAccessor.BaseUrl + _urlAccessor.GetControllerPath("account"),
            newUser.Email
        );

        if (!result)
        {
            throw new AuthenticationErrorException("Something went wrong! Please try again...");
        }
    }

    public async Task<AuthenticatedUserResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        var user = await _userRepository.GetByEmail(loginUserDto.Email) ??
                   throw new InvalidAuthenticationParamsException("Invalid username or password");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new InvalidAuthenticationParamsException("Invalid username or password");
        }

        return await _authenticator.Authenticate(user);
    }

    public async Task<AuthenticatedUserResponseDto> RefreshUserJwt(string refreshToken)
    {
        var isValidToken = _refreshTokenValidator.Validate(refreshToken);
        if (!isValidToken)
        {
            throw new InvalidAuthenticationParamsException("Could not use refresh token");
        }

        var refreshTokenDto = await _refreshTokenRepository.GetByToken(refreshToken) ??
                              throw new InvalidAuthenticationParamsException("Could not use refresh token");

        await _refreshTokenRepository.Delete(refreshTokenDto.Id);

        var user = await _userRepository.GetById(refreshTokenDto.UserId) ??
                   throw new AuthenticationErrorException("Authentication error");

        var response = await _authenticator.Authenticate(user);
        return response;
    }

    public async Task DeleteUserRefreshTokens()
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
        {
            throw new AuthenticationErrorException("You need to be logged in to perform this action");
        }

        await _refreshTokenRepository.DeleteByUserId(userId.Value);
    }

    public async Task ResendUserVerificationEmail()
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
        {
            throw new AuthenticationErrorException("You need to be logged in to perform this action");
        }

        var verificationTokenDto = await _verificationTokenRepository.GetByUserId(userId.Value) ??
                                   throw new InvalidAuthenticationParamsException(
                                       "Could not resend verification token");

        var user = await _userRepository.GetById(userId.Value) ??
                   throw new AuthenticationErrorException("Authentication error");

        var hashedToken = _encryptionHelper.Encrypt(verificationTokenDto.VerificationToken);
        var result = await _emailAuthenticationHelper.SendVerificationEmail(
            hashedToken,
            _urlAccessor.BaseUrl + _urlAccessor.GetControllerPath("account"),
            user.Email
        );

        if (!result)
        {
            throw new AuthenticationErrorException("Something went wrong! Please try again...");
        }
    }

    public async Task VerifyUser(string tokenHash)
    {
        var token = _encryptionHelper.Decrypt(tokenHash);
        var verificationTokenDto = await _verificationTokenRepository.GetByToken(token) ??
                                   throw new AuthenticationErrorException("Verification error");

        var user = await _userRepository.GetById(verificationTokenDto.UserId);
        if (user is not null && user.IsVerified)
        {
            throw new AuthenticationErrorException("User already verified");
        }

        var updateStatus = _userRepository.UpdateUserVerificationStatusTrue(verificationTokenDto.UserId);
        var deleteVerificationToken = _verificationTokenRepository.DeleteByUserId(verificationTokenDto.UserId);

        await Task.WhenAll(updateStatus, deleteVerificationToken);
    }

    public async Task<UserDetailsDto?> GetUserDetails()
    {
        var userId = _userAccessor.UserId;
        if (!userId.HasValue)
        {
            throw new AuthenticationErrorException("You need to be logged in to perform this action");
        }

        var user = await _userRepository.GetById(userId.Value);
        if (user is null)
        {
            throw new AuthenticationErrorException("Authentication error");
        }

        return new UserDetailsDto
        {
            Email = user.Email,
            Username = user.Username,
            PhoneNumber = user.PhoneNumber,
            Name = user.Name,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            AccountType = user.Role.Name,
            IsVerified = user.IsVerified
        };
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