using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Skill
{
	[Key]
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Level { get; set; }

	public List<PortfolioUser> PortfolioUsers { get; set; }

}
