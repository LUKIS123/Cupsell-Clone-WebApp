namespace CupsellCloneAPI.Database.Authentication.Models
{
    public class RefreshTokenDto
    {
        public required Guid Id { get; set; }
        public required string RefreshToken { get; set; }
        public required Guid UserId { get; set; }
    }
}