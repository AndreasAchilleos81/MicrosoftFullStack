using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	[Table("Registration")]
	public class Registration
	{
		[Key]
		[Column("UserId")]
		public string UserId { get; set; }

		[Column("registered_at")]
		public DateTime RegisteredAt { get; set; }

		[Column("terminated_at")]
		public DateTime? TerminatedAt { get; set; }

	}
}
