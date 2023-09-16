using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class Offer
    {
        [Key] public required Guid Id { get; set; }
        [ForeignKey("Product")] public required Guid ProductId { get; set; }
        [ForeignKey("Graphic")] public required Guid GraphicId { get; set; }
        [ForeignKey("Seller")] public required Guid SellerId { get; set; }
        public required decimal Price { get; set; }
        public required bool IsAvailable { get; set; }

        public virtual required Product Product { get; set; }
        public virtual required Graphic Graphic { get; set; }
        public virtual required User.User Seller { get; set; }
    }
}