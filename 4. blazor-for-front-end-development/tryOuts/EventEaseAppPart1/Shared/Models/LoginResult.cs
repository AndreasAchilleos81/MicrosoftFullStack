namespace Shared.Models
{
	public class LoginResult
	{
		public bool Succeeded { get; set; }
		public IEnumerable<string> Errors { get; set; }
	}
}
