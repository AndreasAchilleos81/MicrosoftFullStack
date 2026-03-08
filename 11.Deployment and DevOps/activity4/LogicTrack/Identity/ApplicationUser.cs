using Microsoft.AspNetCore.Identity;

namespace LogicTrack.Identity
{
	public class ApplicationUser 
		: IdentityUser
	{
		public ApplicationUser() { }


		public ApplicationUser(string userName, string email) : base(userName)
		{
			Email = email;
		}
	}
}
