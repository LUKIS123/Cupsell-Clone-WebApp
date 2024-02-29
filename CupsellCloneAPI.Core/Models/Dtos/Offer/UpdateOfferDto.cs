namespace CupsellCloneAPI.Core.Models.Dtos.Offer
{
    public class UpdateOfferDto
    {
        public Guid OfferId { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? GraphicId { get; set; }
        public decimal? Price { get; set; }
        public bool? IsAvailable { get; set; }
        public string? Description { get; set; }
    }
}