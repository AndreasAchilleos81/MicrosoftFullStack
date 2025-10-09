using SimpleTokenAuthApp;

var authManager = new AuthManager();
var secureContentManager = new SecureContentManager(authManager);

Console.WriteLine("Welcome to the Simple Token Auth App!");
while (true)
{
    Console.WriteLine(
        "\nChoose an option: \n1. Register \n2. Login \n3. Access Secure Content \n4. Exit"
    );
    string choice = Console.ReadLine()!;
    switch (choice)
    {
        case "1":
            authManager.Register();
            break;
        case "2":
            authManager.Login();
            break;
        case "3":
            secureContentManager.AccessSecureContent();
            break;
        case "4":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}
