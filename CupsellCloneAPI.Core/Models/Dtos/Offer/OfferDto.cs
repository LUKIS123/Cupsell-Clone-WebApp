using CupsellCloneAPI.Core.Models.Dtos.Graphic;
using CupsellCloneAPI.Core.Models.Dtos.Product;
using CupsellCloneAPI.Core.Models.Dtos.User;

namespace CupsellCloneAPI.Core.Models.Dtos.Offer
{
    public class OfferDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }

        public ProductDto? Product { get; set; }
        public GraphicDto? Graphic { get; set; }
        public UserDto? Seller { get; set; }

        public Dictionary<SizeDto, int> SizeQuantityDictionary { get; set; } = new();
        public IEnumerable<string> ImageUrls { get; set; } = default!;
    }
}