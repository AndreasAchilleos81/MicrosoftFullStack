using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Project
{
	[Key]
	public int Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	public string ImageUrl { get; set; }
	
	public List<PortfolioUser> PortfolioUsers { get; set; }

}
