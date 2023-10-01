using System.Text;
using System.Security.Cryptography;
using CupsellCloneAPI.Core.Utils.Encryption.Settings;

namespace CupsellCloneAPI.Core.Utils.Encryption
{
    public class DefaultEncryptionHelper : IEncryptionHelper
    {
        private readonly EncryptionSettings _settings;

        public DefaultEncryptionHelper(EncryptionSettings encryptionSettings)
        {
            _settings = encryptionSettings;
        }

        public string Encrypt(string clearText)
        {
            var clearBytes = Encoding.Unicode.GetBytes(clearText);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(_settings.EncryptionKey, _settings.Salt, 10000, HashAlgorithmName.SHA1);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();

            clearText = Convert.ToBase64String(ms.ToArray());
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(_settings.EncryptionKey, _settings.Salt, 10000, HashAlgorithmName.SHA1);
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);

            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(cipherBytes, 0, cipherBytes.Length);
            cs.Close();

            cipherText = Encoding.Unicode.GetString(ms.ToArray());
            return cipherText;
        }
    }
}