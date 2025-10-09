namespace SimpleTokenAuthApp;

public class SecureContentManager
{
    private readonly AuthManager _authManager;

    public SecureContentManager(AuthManager authManager)
    {
        _authManager = authManager;
    }

    public void AccessSecureContent()
    {
        Console.Write("Enter your token to access secure content: ");
        string token = Console.ReadLine() ?? string.Empty;

        var user = _authManager.GetUserByToken(token);
        if (user == null)
        {
            Console.WriteLine("Access denied. Invalid or missing token.");
            return;
        }

        Console.WriteLine($"Access granted. Welcome, {user.Username}!");
        // Here you can add more secure content logic
    }
}
