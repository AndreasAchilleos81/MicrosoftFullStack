namespace Shared.Result
{
	public class RegistrationResult
	{
		public IEnumerable<string> Errors { get; set; } = new List<string>();
		public bool Succeeded { get; set; }
	}
}
