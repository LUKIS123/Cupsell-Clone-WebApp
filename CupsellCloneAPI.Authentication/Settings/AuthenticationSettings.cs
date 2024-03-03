namespace CupsellCloneAPI.Authentication.Settings
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; } = default!;
        public string RefreshTokenKey { get; set; } = default!;
        public int JwtExpireMinutes { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }
        public string JwtIssuer { get; set; } = default!;
        public string JwtAudience { get; set; } = default!;
    }
}