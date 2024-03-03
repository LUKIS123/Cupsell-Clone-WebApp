namespace CupsellCloneAPI.EmailCommunication.Settings
{
    public class SmtpEmailSettings
    {
        public string SmtpHostname { get; set; } = default!;
        public int Port { get; set; }
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string SenderName { get; set; } = default!;
    }
}