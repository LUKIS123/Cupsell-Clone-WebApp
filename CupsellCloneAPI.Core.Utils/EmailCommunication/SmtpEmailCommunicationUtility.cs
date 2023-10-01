using CupsellCloneAPI.Core.Utils.EmailCommunication.Settings;
using MimeKit;

namespace CupsellCloneAPI.Core.Utils.EmailCommunication;

public class SmtpEmailCommunicationUtility : IEmailCommunicationUtility
{
    private readonly SmtpEmailSettings _settings;

    public SmtpEmailCommunicationUtility(SmtpEmailSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> SendMailAsync(string htmlBody, string subject, string receiverEmailAddress)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_settings.SenderName, _settings.Username));
        emailMessage.To.Add(MailboxAddress.Parse(receiverEmailAddress));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody,
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
        string response;
        try
        {
            await smtpClient.ConnectAsync(_settings.SmtpHostname, _settings.Port, false);
            smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
            await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);
            response = await smtpClient.SendAsync(emailMessage);
        }
        catch
        {
            response = string.Empty;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
            smtpClient.Dispose();
        }

        // Sent == Great success
        return response.ToLower().Contains("success");
    }
}