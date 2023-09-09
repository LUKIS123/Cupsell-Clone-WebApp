using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CupsellCloneAPI.Database.Entities.Product;

namespace CupsellCloneAPI.Database.Entities.Order
{
    public class OrderItem
    {
        [Key] public required Guid Id { get; set; }
        [ForeignKey("Order")] public required Guid OrderId { get; set; }
        [ForeignKey("Offer")] public required Guid OfferId { get; set; }
        [ForeignKey("Size")] public required int SizeId { get; set; }
        public required int Quantity { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Offer? Offer { get; set; }
        public virtual Size? Size { get; set; }
    }
}

// Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
//     OrderId UNIQUEIDENTIFIER NOT NULL,
//     FOREIGN KEY (OrderId) REFERENCES [orders].[Orders] (Id),
// OfferId UNIQUEIDENTIFIER NOT NULL,
//     FOREIGN KEY (OfferId) REFERENCES [products].[Offers] (Id),
// SizeId INT NOT NULL,
//     FOREIGN KEY (SizeId) REFERENCES [products].[Sizes] (Id),
// Quantity INT NOT NULL,