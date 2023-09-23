namespace CupsellCloneAPI.Database.BlobContainer.Models
{
    public class BlobObject
    {
        public required Stream FileStream { get; set; }
        public required string ContentType { get; set; }
    }
}