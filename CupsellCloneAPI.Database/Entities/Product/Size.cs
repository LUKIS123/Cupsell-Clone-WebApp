using System.ComponentModel.DataAnnotations;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class Size
    {
        [Key] public required int Id { get; set; }
        public required string Name { get; set; }
    }
}