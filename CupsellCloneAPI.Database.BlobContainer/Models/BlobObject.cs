namespace CupsellCloneAPI.Database.BlobContainer.Models
{
    public class BlobObject
    {
        public Stream? FileStream { get; set; }
        public string? ContentType { get; set; }
    }
}