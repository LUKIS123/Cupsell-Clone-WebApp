namespace CupsellCloneAPI.Core.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public required string RoleName { get; set; }
    }
}