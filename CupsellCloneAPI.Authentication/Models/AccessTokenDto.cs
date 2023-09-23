namespace CupsellCloneAPI.Authentication.Models
{
    public class AccessTokenDto
    {
        public required string Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}