namespace Shared.Models;

public class PortfolioUser
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Bio { get; set; }
	public string ProfilePictureUrl { get; set; }

	public List<Skill> Skills { get; set; } = new();
	public List<Project> Projects { get; set; } = new();
}