namespace CupsellCloneAPI.Core.Models.Dtos.Graphic
{
    public class GraphicDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string SellerName { get; set; }
        public string? BlobUrl { get; set; }
    }
}