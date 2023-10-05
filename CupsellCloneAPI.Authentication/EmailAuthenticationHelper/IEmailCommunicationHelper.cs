namespace CupsellCloneAPI.Authentication.EmailAuthenticationHelper
{
    public interface IEmailCommunicationHelper
    {
        Task<bool> SendVerificationEmail(string hashedToken, string baseActionUrl, string emailAddress);
    }
}