using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
	public class EventCard
	{
		public string Id { get; set; }

		[Required(ErrorMessage = "Name of event is Required")]
		[StringLength(30, MinimumLength = 3, ErrorMessage = "Name of event must between 3 to 30 chars")]
		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		[Required(ErrorMessage = "Location is required")]
		public string Location { get; set; } = string.Empty;

		[Required(ErrorMessage = "Access type to Event is required")]
		[StringLength(50, MinimumLength = 10, ErrorMessage = "Location of event must between 10 to 50 chars")]
		public bool IsPublic { get; set; }

		[Range(10, 100, ErrorMessage = "From 10 to 100 attendees to host an event")]
		public int MaxAttendees { get; }

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
