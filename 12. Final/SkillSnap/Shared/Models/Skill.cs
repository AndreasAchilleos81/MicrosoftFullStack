using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class Skill
{
	[Key]
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Level { get; set; }

	[ForeignKey("PortfolioUserId")]
	public int PortfolioUserId { get; set; }
}
