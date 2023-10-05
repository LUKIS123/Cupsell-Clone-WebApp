namespace CupsellCloneAPI.Database.Authentication.Models
{
    public class VerificationTokenDto
    {
        public required Guid UserId { get; set; }
        public required string VerificationToken { get; set; }
    }
}