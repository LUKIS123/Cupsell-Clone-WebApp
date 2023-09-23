namespace CupsellCloneAPI.Authentication.Exceptions
{
    public class InvalidLoginParamsException : Exception
    {
        public InvalidLoginParamsException(string? message) : base(message)
        {
        }
    }
}