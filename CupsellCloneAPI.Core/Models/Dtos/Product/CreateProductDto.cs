namespace CupsellCloneAPI.Core.Models.Dtos.Product
{
    public class CreateProductDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string ProductTypeName { get; set; }
    }
}