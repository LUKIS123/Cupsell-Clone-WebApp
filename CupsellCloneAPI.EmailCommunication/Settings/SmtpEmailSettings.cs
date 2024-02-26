namespace CupsellCloneAPI.EmailCommunication.Settings
{
    public class SmtpEmailSettings
    {
        public string SmtpHostname { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SenderName { get; set; }
    }
}