using Microsoft.Extensions.Primitives;

public class Configuration
{
    private readonly IConfiguration _configuration;
    public Configuration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Token => _configuration["token"];
}