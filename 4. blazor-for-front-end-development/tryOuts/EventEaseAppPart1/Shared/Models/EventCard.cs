using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
	[Table("EventCard")]
	public class EventCard
	{
		public EventCard()
		{
			
		}

		[Key]
		[Column("Id")]
		public string Id { get; set; }


		[Column("Name")]
		[Required(ErrorMessage = "Event name is required")]
		public string Name { get; set; } = string.Empty;


		[Column("Description")]
		[StringLength(300, MinimumLength = 3, ErrorMessage = "Description of event must between 3 to 300 chars")]
		public string Description { get; set; } = string.Empty;

		[Column("Location")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Location of event must between 3 to 30 chars")]
		public string Location { get; set; } = string.Empty;

		[Column("IsPublic")]
		public bool IsPublic { get; set; }

		[Column("MaxAttendees")]
		[Range(10, 100, ErrorMessage = "From 10 to 100 attendees to host an event")]
		public int MaxAttendees { get; } = 100;

		[Column("CurrentAttendees")]
		public int CurrentAttendees { get; set; }

		public void Attend(int attendees = 1)
		{
			if ((CurrentAttendees + attendees) > MaxAttendees)
			{
				throw new InvalidOperationException($"Cannot attend more than the maximum number ({MaxAttendees}) of attendees. Current Attendance at: {CurrentAttendees}");
			}
		}

		public override string ToString()
		{
			return $"EventCard: {Id}, Name: {Name}, Description: {Description}, Location: {Location}, IsPublic: {IsPublic}, MaxAttendees: {MaxAttendees}, CurrentAttendees: {CurrentAttendees}";
		}
	}
}
