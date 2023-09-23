namespace CupsellCloneAPI.Database.BlobContainer.Exceptions
{
    public class BlobFileNotFoundException : Exception
    {
        public BlobFileNotFoundException(string? message) : base(message)
        {
        }
    }
}