using CupsellCloneAPI.Core.Utils.EmailCommunication;

namespace CupsellCloneAPI.Authentication.EmailAuthenticationHelper
{
    public class EmailCommunicationHelper : IEmailCommunicationHelper
    {
        private readonly IEmailCommunicationUtility _emailCommunicationUtility;

        const string VerificationMessageTemplate = @"
<p>Hello user, please verify your account by clicking the link below:</p>
<a href=""#URL#""> <h3>""#URL#""</h3></a>";

        public EmailCommunicationHelper(IEmailCommunicationUtility emailCommunicationUtility)
        {
            _emailCommunicationUtility = emailCommunicationUtility;
        }

        public async Task<bool> SendVerificationEmail(string hashedToken, string baseActionUrl, string emailAddress)
        {
            var callbackUrl = baseActionUrl + $"verify?encryptedToken={hashedToken}";
            var body = VerificationMessageTemplate.Replace("#URL#",
                System.Text.Encodings.Web.HtmlEncoder.Default.Encode(callbackUrl));

            return await _emailCommunicationUtility.SendMailAsync(
                body,
                "Cupsellclone Account Verification",
                emailAddress
            );
        }
    }
}