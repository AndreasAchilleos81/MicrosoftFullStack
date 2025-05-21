using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	[Table("User")]
	public class User
	{
		[Key]
		[Column("Id")]
		public string Id { get; set; }

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

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; } = string.Empty;	

		public override string ToString()
		{
			return $"Id: {Id}, Name: {Name}, LastName: {LastName}, Email: {Email}";
		}
	}
}
