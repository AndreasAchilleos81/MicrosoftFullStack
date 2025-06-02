using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	[Table("Sessions")]
	public class Session
	{
		[Key]
		[Column("UserId")]
		public string UserId { get; set; }

		[Column("start_at")]
		public DateTime StartAt { get; set; }

		[Column("end_time")]
		public DateTime? EndTime{ get; set; }
	}
}
