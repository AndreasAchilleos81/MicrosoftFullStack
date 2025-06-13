using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	[Table("Attendance")]
	public class Attendance
	{
		public Attendance()
		{
		}

		[Key]
		[Column("EventId")]
		public string EventId { get; set; }

		[Key]
		[Column("UserId")]
		public string UserId { get; set; }

		// null for not intested/going, true for going, false for interested
		[Column("Attended")]
		public bool? Attended { get; set; }
	}

	public static class AttendanceStatus
	{
		public static bool Going => true;
		public static bool Interested => false;

		public static bool? NotInterestedOrGoing => null;
	}

}
