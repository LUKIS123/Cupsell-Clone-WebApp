using System.ComponentModel.DataAnnotations;

namespace CupsellCloneAPI.Database.Entities.Product
{
    public class ProductType
    {
        [Key] public required int Id { get; set; }
        public required string Name { get; set; }
    }
}