using System.Text.Json.Serialization;

namespace CupsellCloneAPI.Core.Utils.EmailCommunication.Settings
{
    public class ApiEmailSettings
    {
        public string BaseUrl { get; set; }
        public string Username { get; set; }
        public string ApiKey { get; set; }
        public string DomainName { get; set; }
        public string SenderName { get; set; }
    }
}