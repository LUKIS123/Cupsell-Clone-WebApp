namespace CupsellCloneAPI.Authentication.Models
{
    public class UserDetailsDto
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public required string AccountType { get; set; }
        public required bool IsVerified { get; set; }
    }
}