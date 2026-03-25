namespace Shared.Models.DTO;

public class Profiles
{
    //public ListOfPortfolioUser value { get; set; }

    public List<PortfolioUserValue> value { get; set; } = new();

    public string executionMs { get; set; }
}

public class PortfolioUserValue
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public List<Skill> Skills { get; set; } = new();
    public List<Project> Projects { get; set; } = new();
}
