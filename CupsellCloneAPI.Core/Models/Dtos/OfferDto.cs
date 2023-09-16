namespace CupsellCloneAPI.Core.Models.Dtos
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public ProductDto? Product { get; set; }
        public GraphicDto? Graphic { get; set; }
        public UserDto? Seller { get; set; }

        public Dictionary<string, int> SizeQuantityDictionary { get; set; } = new();
        public IEnumerable<string> ImageUrls { get; set; } = default!;
    }
}