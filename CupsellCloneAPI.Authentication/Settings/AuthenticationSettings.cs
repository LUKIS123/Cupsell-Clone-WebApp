namespace CupsellCloneAPI.Authentication.Settings
{
    public class AuthenticationSettings
    {
        public string JwtKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public int JwtExpireMinutes { get; set; }
        public int RefreshTokenExpireMinutes { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
    }
}