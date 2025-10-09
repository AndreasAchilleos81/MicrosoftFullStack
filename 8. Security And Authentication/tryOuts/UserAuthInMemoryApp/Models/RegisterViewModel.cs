using System.ComponentModel.DataAnnotations;

namespace UserAuthInMemoryApp.Models;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [StringLength(
        100,
        ErrorMessage = "Password must be at least 6 character long",
        MinimumLength = 6
    )]
    [Display(Name = "Password")]
    public string Password { get; set; }
}
