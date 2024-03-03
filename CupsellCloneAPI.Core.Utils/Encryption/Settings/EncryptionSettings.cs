namespace CupsellCloneAPI.Core.Utils.Encryption.Settings
{
    public class EncryptionSettings
    {
        public required string EncryptionKey { get; set; }
        public required byte[] Salt { get; set; }
    }
}