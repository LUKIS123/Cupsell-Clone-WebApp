using CupsellCloneAPI.EmailCommunication.Settings;
using RestSharp;
using RestSharp.Authenticators;

namespace CupsellCloneAPI.EmailCommunication.EmailCommunication;

public class ApiEmailCommunicationUtility : IEmailCommunicationUtility
{
    private readonly ApiEmailSettings _settings;

    public ApiEmailCommunicationUtility(ApiEmailSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> SendMailAsync(string htmlBody, string subject, string receiverEmailAddress)
    {
        var client = new RestClient(_settings.BaseUrl, options =>
            options.Authenticator = new HttpBasicAuthenticator(_settings.SenderName, _settings.ApiKey)
        );

        var request = new RestRequest();
        request.AddParameter("domain", _settings.DomainName, ParameterType.UrlSegment);
        request.Resource = "{domain}/messages";
        request.AddParameter("from", $"{_settings.SenderName} <{_settings.Username}>");
        request.AddParameter("to", receiverEmailAddress);
        request.AddParameter("subject", subject);
        request.AddParameter("text/xml", htmlBody, ParameterType.RequestBody);
        request.Method = Method.Post;
        var response = await client.ExecuteAsync(request);

        return response.IsSuccessful;
    }
}