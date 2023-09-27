namespace CupsellCloneAPI.Authentication.Exceptions
{
    public class AuthenticationErrorException : Exception
    {
        public AuthenticationErrorException(string? message) : base(message)
        {
        }
    }
}