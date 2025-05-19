using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	public class User
	{
		[Key]
		[Column("Id")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Username is required")]
		[Column("Name")]
		public string Name { get; set; } = string.Empty;

		[Required(ErrorMessage ="Last Name is required")]
		[Column("LastName")]
		public string LastName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		[Column("Email")]
		public string Email { get; set; } = string.Empty;


	}
}
