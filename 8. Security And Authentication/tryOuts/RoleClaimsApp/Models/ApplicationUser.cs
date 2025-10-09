using Microsoft.AspNetCore.Identity;

namespace RoleClaimsApp.Models;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
