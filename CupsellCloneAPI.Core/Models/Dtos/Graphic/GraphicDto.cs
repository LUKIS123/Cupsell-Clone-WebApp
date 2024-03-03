namespace CupsellCloneAPI.Core.Models.Dtos.Graphic
{
    public class GraphicDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string SellerName { get; set; }
        public string? Description { get; set; }
        public string? ImageUri { get; set; }
    }
}