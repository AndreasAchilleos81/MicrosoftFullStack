using System.Security.Cryptography;
using SecureApp.Models;

namespace SecureApp;

public class Program
{
    public static void Main(string[] args)
    {
        // var user = new User { Username = "adminUser", Role = "Admin" };
        var user = new User { Username = "regularUser", Role = "User" };

        var storage = new SecureStorage();
        byte[] encryptionKey;
        byte[] initializationVector;

        using (var aes = Aes.Create())
        {
            aes.GenerateKey();
            aes.GenerateIV();
            encryptionKey = aes.Key;
            initializationVector = aes.IV;

            storage.StoreData("Sensitive Information", encryptionKey, initializationVector);
        }

        try
        {
            var data = storage.RetrieveData(user, encryptionKey, initializationVector);
            Console.WriteLine($"Admin accessed data: {data}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
