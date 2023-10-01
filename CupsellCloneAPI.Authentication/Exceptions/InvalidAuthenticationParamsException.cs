namespace CupsellCloneAPI.Authentication.Exceptions
{
    public class InvalidAuthenticationParamsException : Exception
    {
        public InvalidAuthenticationParamsException(string? message) : base(message)
        {
        }
    }
}