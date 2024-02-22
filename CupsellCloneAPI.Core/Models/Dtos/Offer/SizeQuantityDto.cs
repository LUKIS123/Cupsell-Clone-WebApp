namespace CupsellCloneAPI.Core.Models.Dtos.Offer
{
    public class SizeQuantityDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required int Quantity { get; set; } = 0;
    }
}