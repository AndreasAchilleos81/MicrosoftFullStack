using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
    private string _password;
    public string HashedPassword { get; private set; }

    [JsonConstructor]
    public User(string name, string email)
    {
        Name = name;
        Email = email;
        _password = string.Empty; // Initialize password to an empty string
        HashedPassword = string.Empty; // Initialize HashedPassword to an empty string  
    }   

    public void SetPassword(string password)
    {
        _password = password;
        using SHA256 sha = SHA256.Create();
        HashedPassword = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_password)));
    }   

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        _password = password;
        using SHA256 sha = SHA256.Create();
        HashedPassword = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_password)));
    }

    public bool ValidatePassword()
    {
        using SHA256 sha = SHA256.Create();
        var hashedInput = Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_password)));
        return HashedPassword == hashedInput;
    }

    public string Serialize()
    {
        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(HashedPassword))
        {
            throw new InvalidOperationException("User properties cannot be null or empty.");
        }   
        return JsonSerializer.Serialize(this);
    }

    public User Deserialize(string jsonString, bool trustedSource = true)
    {
        if (trustedSource)
        {
            var user = JsonSerializer.Deserialize<User>(jsonString);
            if (user == null)
            {
                throw new InvalidOperationException("Deserialization failed: result is null.");
            }
            return user;
        }
        else
        {
            throw new InvalidOperationException("Deserialization from untrusted sources is not allowed.");
        }
    }
}