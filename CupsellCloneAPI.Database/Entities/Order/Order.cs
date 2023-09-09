using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CupsellCloneAPI.Database.Entities.Order
{
    public class Order
    {
        [Key] public required Guid Id { get; set; }
        [ForeignKey("User")] public required Guid UserId { get; set; }
        [ForeignKey("OrderStatus")] public required int OrderStatusId { get; set; }

        public virtual User.User? User { get; set; }
        public virtual OrderStatus? OrderStatus { get; set; }
        public virtual List<OrderItem>? OrderItems { get; set; }
    }
}