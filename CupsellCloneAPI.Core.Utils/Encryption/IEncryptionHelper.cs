namespace CupsellCloneAPI.Core.Utils.Encryption;

public interface IEncryptionHelper
{
    string Encrypt(string clearText);
    string Decrypt(string cipherText);
}