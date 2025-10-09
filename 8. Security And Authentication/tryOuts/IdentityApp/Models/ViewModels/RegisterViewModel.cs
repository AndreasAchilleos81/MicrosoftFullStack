using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Models.ViewModels;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "First Name")]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
