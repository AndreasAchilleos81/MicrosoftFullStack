using System.ComponentModel.DataAnnotations;

namespace LogicTrack.Identity
{
    public class RegistrationUser
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public ApplicationUser ToIdentityUser()
        {
            return new ApplicationUser(UserName, Email);
        }
    }
}
