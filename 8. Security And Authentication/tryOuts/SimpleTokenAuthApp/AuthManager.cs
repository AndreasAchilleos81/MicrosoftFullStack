namespace SimpleTokenAuthApp;

public class AuthManager
{
    private readonly List<User> _users = new();
    private readonly TokenManager _tokenManager = new();

    public void Register()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter password: ");
        string password = Console.ReadLine() ?? string.Empty;

        var user = new User { Username = username, Password = password };
        if (_users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return;
        }

        _users.Add(user);
        Console.WriteLine("User registered successfully.");
    }

    public void Login()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine() ?? string.Empty;

        Console.Write("Enter password: ");
        string password = Console.ReadLine() ?? string.Empty;

        var user = _users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user == null)
        {
            Console.WriteLine("Invalid username or password.");
            return;
        }

        user.Token = _tokenManager.GenerateToken();
        Console.WriteLine($"Login successful. Your token: {user.Token}");
    }

    public User? GetUserByToken(string token)
    {
        return _users.FirstOrDefault(u => u.Token == token);
    }
}
