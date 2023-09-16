using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.User
{
    public class User
    {
        [Key] public required Guid Id { get; set; }
        [EmailAddress] public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        [Phone] public required string PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        [ForeignKey("Role")] public required int RoleId { get; set; }

        public virtual required Role Role { get; set; }
    }
}