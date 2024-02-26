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
        [ForeignKey("Seller")] public required Guid SellerId { get; set; }

        public virtual required ProductType ProductType { get; set; }
        public virtual User.User? Seller { get; set; }
    }
}