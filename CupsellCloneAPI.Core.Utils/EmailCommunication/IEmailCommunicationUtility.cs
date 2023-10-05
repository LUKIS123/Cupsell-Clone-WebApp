namespace CupsellCloneAPI.Core.Utils.EmailCommunication
{
    public interface IEmailCommunicationUtility
    {
        Task<bool> SendMailAsync(string htmlBody, string subject, string receiverEmailAddress);
    }
}