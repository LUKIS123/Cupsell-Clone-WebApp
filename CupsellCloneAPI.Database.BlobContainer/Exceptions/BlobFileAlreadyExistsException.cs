namespace CupsellCloneAPI.Database.BlobContainer.Exceptions
{
    public class BlobFileAlreadyExistsException : Exception
    {
        public BlobFileAlreadyExistsException(string? message) : base(message)
        {
        }
    }
}