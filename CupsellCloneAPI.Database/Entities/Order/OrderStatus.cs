using System.ComponentModel.DataAnnotations;

namespace CupsellCloneAPI.Database.Entities.Order
{
    public class OrderStatus
    {
        [Key] public required int Id { get; set; }
        public required string Name { get; set; }
    }
}