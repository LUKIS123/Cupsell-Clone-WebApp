namespace CupsellCloneAPI.EmailCommunication.Settings
{
    public class ApiEmailSettings
    {
        public string BaseUrl { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string ApiKey { get; set; } = default!;
        public string DomainName { get; set; } = default!;
        public string SenderName { get; set; } = default!;
    }
}