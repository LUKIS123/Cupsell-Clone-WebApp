namespace CupsellCloneAPI.Core.Models.Dtos.Offer
{
    public class CreateOfferDto
    {
        public required Guid ProductId { get; set; }
        public required Guid GraphicsId { get; set; }
        public required decimal Price { get; set; }
        public required Dictionary<int, int> SizeIdQuantityDictionary { get; set; }
    }
}