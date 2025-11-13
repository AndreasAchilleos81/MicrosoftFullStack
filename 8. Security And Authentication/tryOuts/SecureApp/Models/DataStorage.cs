using System.Security.Cryptography;

namespace SecureApp.Models
{
    public class SecureStorage
    {
        private string _encryptedData;

        public void StoreData(string data, byte[] key, byte[] iv)
        {
            _encryptedData = Convert.ToBase64String(Encrypt(data, key, iv));
        }

        public string RetrieveData(User user, byte[] key, byte[] iv)
        {
            if (user.Role != "Admin")
            {
                throw new UnauthorizedAccessException(
                    "User does not have permission to access this data."
                );
            }

            return Decrypt(key, iv);
        }

        private byte[] Encrypt(string data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(data);
                    }
                    return ms.ToArray();
                }
            }
        }

        public string Decrypt(byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                var cipherText = Convert.FromBase64String(_encryptedData);

                using (var ms = new MemoryStream(cipherText))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
