using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class AvailableItem
    {
        [Key] public required Guid Id { get; set; }
        [ForeignKey("Offer")] public required Guid OfferId { get; set; }
        [ForeignKey("Size")] public required int SizeId { get; set; }
        public required int Quantity { get; set; }

        public virtual required Size Size { get; set; }
    }
}