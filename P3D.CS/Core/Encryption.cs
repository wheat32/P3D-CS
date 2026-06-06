using System.Security.Cryptography;
using System.Text;

namespace P3D;

internal static class Encryption
{
    private const int IV_LENGTH = 16;

    public static String EncryptString(String s, String password)
    {
        byte[] key = MD5.HashData(Encoding.UTF8.GetBytes(StringObfuscation.DeObfuscate(password)));

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.GenerateIV();

        using MemoryStream ms = new MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            byte[] data = Encoding.UTF8.GetBytes(s);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static String DecryptString(String s, String password)
    {
        byte[] key = MD5.HashData(Encoding.UTF8.GetBytes(StringObfuscation.DeObfuscate(password)));
        byte[] encdata = Convert.FromBase64String(s);

        using MemoryStream ms = new MemoryStream(encdata);
        byte[] iv = new byte[IV_LENGTH];
        ms.Read(iv, 0, iv.Length);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        byte[] buf = new byte[ms.Length - IV_LENGTH];
        int read = cs.Read(buf, 0, buf.Length);
        return Encoding.UTF8.GetString(buf, 0, read);
    }
}
