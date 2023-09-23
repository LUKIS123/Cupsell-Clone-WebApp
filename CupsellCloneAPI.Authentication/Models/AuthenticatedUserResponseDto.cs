namespace CupsellCloneAPI.Authentication.Models
{
    public class AuthenticatedUserResponseDto
    {
        public required string AccessToken { get; set; }
        public required DateTime AccessTokenExpirationTime { get; set; }
        public required string RefreshToken { get; set; }
    }
}