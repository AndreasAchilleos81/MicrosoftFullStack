using System.Security.Cryptography;

namespace EncryptionApp.Services;

public class EncryptionService
{
    private readonly byte[] _key = Convert.FromBase64String(
        "oe42Bkywge9MD8Qe5fGS75CvVC5eqadhdFC87Z5B1zw="
    );
    private readonly byte[] _iv = Convert.FromBase64String("Qo7jOxADSxMWBJF8xgPvoA==");

    public void EncryptFile(string inputPath, string outputPath)
    {
        using var aes = Aes.Create();
        using var encryptor = aes.CreateEncryptor(_key, _iv);
        using var inputStream = new FileStream(inputPath, FileMode.Open);
        using var outputStream = new FileStream(outputPath, FileMode.Create);
        using var cryptoStream = new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);
        inputStream.CopyTo(cryptoStream);
    }

    public void DecryptFile(string inputPath, string outputPath)
    {
        using var aes = Aes.Create();
        using var decryptor = aes.CreateDecryptor(_key, _iv);
        using var inputStream = new FileStream(inputPath, FileMode.Open);
        using var outputStream = new FileStream(outputPath, FileMode.Create);
        using var cryptoStream = new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
        cryptoStream.CopyTo(outputStream);
    }
}
