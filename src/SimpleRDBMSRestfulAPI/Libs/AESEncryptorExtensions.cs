using System.Security.Cryptography;
using System.Text;
namespace SimpleRDBMSRestfulAPI.Libs;

public static class AESEncryptorExtensions
{
    public static byte[] EncryptAES(this string plainText, string key)
    {
        using Aes aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
        aes.IV = new byte[16]; // Default IV of 16 bytes (zero-initialized)

        using var ms = new MemoryStream();
        using (var encryptor = aes.CreateEncryptor())
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
            sw.Flush(); // Ensure all data is written to the underlying streams
        }

        return ms.ToArray(); // Convert encrypted data in MemoryStream to byte array
    }

    public static string DecryptAES(this byte[] cipher, string key)
    {
        using Aes aes = Aes.Create();

        aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
        aes.IV = new byte[16];

        using var decryptor = aes.CreateDecryptor();
        using var ms = new MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}