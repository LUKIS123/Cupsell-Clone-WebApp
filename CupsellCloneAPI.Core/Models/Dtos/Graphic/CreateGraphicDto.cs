namespace CupsellCloneAPI.Core.Models.Dtos.Graphic
{
    public class CreateGraphicDto
    {
        public required string Name { get; set; }
        public required Guid SellerId { get; set; }
    }
}