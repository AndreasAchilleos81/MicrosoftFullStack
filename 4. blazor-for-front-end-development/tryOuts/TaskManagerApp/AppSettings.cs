public class AppSettings
{
    private IConfiguration _configuration { get; set; }

    public AppSettings(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetBaseUrl()
    {
        return _configuration["ApiSettings:BaseUrl"]!;
    }
}