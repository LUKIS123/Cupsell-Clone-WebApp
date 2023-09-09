using System.ComponentModel.DataAnnotations;

namespace CupsellCloneAPI.Database.Entities.User
{
    public class Role
    {
        [Key] public required int Id { get; set; }
        public required string Name { get; set; }
    }
}