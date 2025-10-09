using System.Text;

namespace SimpleTokenAuthApp;

public class TokenManager
{
    public string GenerateToken()
    {
        string tokenData =
            @"{
                        ""alg"": ""HS256"",
                        ""typ"": ""JWT""
                        ""Role"": ""Admin""
                        ""exp"": 1711927200                
                            }";

        var bytes = Encoding.UTF8.GetBytes(tokenData);
        return Convert.ToBase64String(bytes);
    }
}
