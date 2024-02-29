using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class Graphic
    {
        [Key] public required Guid Id { get; set; }
        public required string Name { get; set; }
        [ForeignKey("Seller")] public required Guid SellerId { get; set; }
        public string? Description { get; set; }

        public virtual required User.User Seller { get; set; }
    }
}