namespace CupsellCloneAPI.Authentication.TokenValidators;

public interface IRefreshTokenValidator
{
    bool Validate(string refreshToken);
}