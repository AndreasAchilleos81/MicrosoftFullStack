namespace UserAuthInMemoryApp.Models;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

public class LoginViewModel
{
    [Required]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }

    public IdentityUser ConvertTo()
    {
        return new IdentityUser { UserName = Email, Email = Email };
    }
}
