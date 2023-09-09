using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class Product
    {
        [Key] public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        [ForeignKey("ProductType")] public required int ProductTypeId { get; set; }

        public virtual ProductType? ProductType { get; set; }
    }
}