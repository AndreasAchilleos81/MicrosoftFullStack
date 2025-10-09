# ASP.NET Core Identity - Registration and Authorization Tutorial

## Table of Contents
1. [Introduction](#introduction)
2. [Project Setup](#project-setup)
3. [Models and DbContext](#models-and-dbcontext)
4. [Configuration](#configuration)
5. [User Registration](#user-registration)
6. [User Authentication (Login/Logout)](#user-authentication-loginlogout)
7. [Authorization](#authorization)
8. [Views and UI](#views-and-ui)
9. [Testing the Application](#testing-the-application)

## Introduction

This tutorial focuses on implementing user registration and authorization in ASP.NET Core using Identity. We'll cover:

- Setting up ASP.NET Core Identity
- Creating custom user registration
- Implementing login/logout functionality
- Basic authorization with attributes
- Role-based authorization
- Custom authorization policies

## Project Setup

### 1. Create New Project

```bash
# Create new MVC project
dotnet new mvc -n IdentityApp
cd IdentityApp

# Add required packages
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 2. Project Structure

```
IdentityApp/
├── Controllers/
│   ├── AccountController.cs
│   ├── HomeController.cs
│   └── SecureController.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Models/
│   ├── ApplicationUser.cs
│   └── ViewModels/
│       ├── RegisterViewModel.cs
│       └── LoginViewModel.cs
├── Views/
│   ├── Account/
│   ├── Home/
│   └── Secure/
└── Program.cs
```

## Models and DbContext

### 1. Create Custom User Model

```csharp
// Models/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
```

### 2. Create DbContext

```csharp
// Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ApplicationUser
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        });
    }
}
```

### 3. Create View Models

```csharp
// Models/ViewModels/RegisterViewModel.cs
using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [Display(Name = "First Name")]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Last Name")]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

// Models/ViewModels/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }
}
```

## Configuration

### 1. Configure Services in Program.cs

```csharp
// Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure application cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
});

// Add MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed default roles and admin user
await SeedDataAsync(app);

app.Run();

// Seed method
static async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create roles
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Create admin user
    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
```

### 2. Connection String in appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=IdentityAppDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## User Registration

### 1. Account Controller

```csharp
// Controllers/AccountController.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    #region Registration

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                // Assign default User role
                await _userManager.AddToRoleAsync(user, "User");

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                return RedirectToAction("Index", "Home");
            }

            // Add errors to ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }

    #endregion

    #region Login/Logout

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, 
                model.Password, 
                model.RememberMe, 
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Helper Methods

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Lockout()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    #endregion
}
```

## User Authentication (Login/Logout)

The login and logout functionality is already implemented in the Account Controller above. Here are the key points:

### Login Process:
1. User submits email and password
2. `SignInManager.PasswordSignInAsync()` validates credentials
3. On success, user is signed in and redirected
4. On failure, appropriate error message is shown

### Logout Process:
1. User clicks logout
2. `SignInManager.SignOutAsync()` clears authentication
3. User is redirected to home page

### Security Features:
- Account lockout after failed attempts
- Anti-forgery token validation
- Secure password validation
- Remember me functionality

## Authorization

### 1. Basic Authorization with Attributes

```csharp
// Controllers/SecureController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class SecureController : Controller
{
    // Requires any authenticated user
    [Authorize]
    public IActionResult Index()
    {
        var userName = User.Identity?.Name;
        ViewBag.Message = $"Hello {userName}, you are authenticated!";
        return View();
    }

    // Requires Admin role
    [Authorize(Roles = "Admin")]
    public IActionResult AdminOnly()
    {
        ViewBag.Message = "This page is for Admins only!";
        return View();
    }

    // Requires User or Admin role
    [Authorize(Roles = "User,Admin")]
    public IActionResult UserArea()
    {
        ViewBag.Message = "This page is for Users and Admins!";
        return View();
    }

    // Using custom policy
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminPolicy()
    {
        ViewBag.Message = "This page uses Admin policy!";
        return View();
    }

    // Multiple authorization requirements
    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "UserOrAdmin")]
    public IActionResult MultipleAuth()
    {
        ViewBag.Message = "This page has multiple authorization requirements!";
        return View();
    }

    // Anonymous access (overrides controller-level [Authorize])
    [AllowAnonymous]
    public IActionResult Public()
    {
        ViewBag.Message = "This page is accessible to everyone!";
        return View();
    }
}
```

### 2. Home Controller with Mixed Access

```csharp
// Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
```

### 3. Authorization in Views

You can also use authorization in Razor views:

```html
<!-- Example in any view -->
@using Microsoft.AspNetCore.Identity
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

@if (SignInManager.IsSignedIn(User))
{
    <p>Hello @User.Identity?.Name!</p>
    
    @if (User.IsInRole("Admin"))
    {
        <p>You are an administrator.</p>
        <a asp-controller="Secure" asp-action="AdminOnly">Admin Panel</a>
    }
    
    @if (User.IsInRole("User"))
    {
        <p>Welcome, regular user!</p>
    }
}
else
{
    <p>Please log in to access member features.</p>
}
```

## Views and UI

### 1. Register View

```html
<!-- Views/Account/Register.cshtml -->
@model RegisterViewModel
@{
    ViewData["Title"] = "Register";
}

<h2>@ViewData["Title"]</h2>

<div class="row">
    <div class="col-md-6">
        <form asp-action="Register" method="post">
            <div asp-validation-summary="All" class="text-danger"></div>
            
            <div class="form-group mb-3">
                <label asp-for="FirstName" class="form-label"></label>
                <input asp-for="FirstName" class="form-control" />
                <span asp-validation-for="FirstName" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="LastName" class="form-label"></label>
                <input asp-for="LastName" class="form-control" />
                <span asp-validation-for="LastName" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="Email" class="form-label"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="Password" class="form-label"></label>
                <input asp-for="Password" class="form-control" />
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="ConfirmPassword" class="form-label"></label>
                <input asp-for="ConfirmPassword" class="form-control" />
                <span asp-validation-for="ConfirmPassword" class="text-danger"></span>
            </div>
            
            <button type="submit" class="btn btn-primary">Register</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 2. Login View

```html
<!-- Views/Account/Login.cshtml -->
@model LoginViewModel
@{
    ViewData["Title"] = "Log in";
}

<h2>@ViewData["Title"]</h2>

<div class="row">
    <div class="col-md-6">
        <form asp-action="Login" asp-route-returnurl="@ViewData["ReturnUrl"]" method="post">
            <div asp-validation-summary="All" class="text-danger"></div>
            
            <div class="form-group mb-3">
                <label asp-for="Email" class="form-label"></label>
                <input asp-for="Email" class="form-control" />
                <span asp-validation-for="Email" class="text-danger"></span>
            </div>
            
            <div class="form-group mb-3">
                <label asp-for="Password" class="form-label"></label>
                <input asp-for="Password" class="form-control" />
                <span asp-validation-for="Password" class="text-danger"></span>
            </div>
            
            <div class="form-check mb-3">
                <input asp-for="RememberMe" class="form-check-input" />
                <label asp-for="RememberMe" class="form-check-label"></label>
            </div>
            
            <button type="submit" class="btn btn-primary">Log in</button>
        </form>
    </div>
</div>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 3. Navigation Layout

```html
<!-- Views/Shared/_Layout.cshtml (navigation part) -->
@using Microsoft.AspNetCore.Identity
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

<nav class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
    <div class="container-fluid">
        <a class="navbar-brand" asp-area="" asp-controller="Home" asp-action="Index">IdentityApp</a>
        <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse">
            <span class="navbar-toggler-icon"></span>
        </button>
        <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
            <ul class="navbar-nav flex-grow-1">
                <li class="nav-item">
                    <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Index">Home</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">Privacy</a>
                </li>
                @if (SignInManager.IsSignedIn(User))
                {
                    <li class="nav-item">
                        <a class="nav-link text-dark" asp-controller="Secure" asp-action="Index">Secure Area</a>
                    </li>
                    @if (User.IsInRole("Admin"))
                    {
                        <li class="nav-item">
                            <a class="nav-link text-dark" asp-controller="Secure" asp-action="AdminOnly">Admin Panel</a>
                        </li>
                    }
                }
            </ul>
            <partial name="_LoginPartial" />
        </div>
    </div>
</nav>
```

### 4. Login Partial

```html
<!-- Views/Shared/_LoginPartial.cshtml -->
@using Microsoft.AspNetCore.Identity
@inject SignInManager<ApplicationUser> SignInManager
@inject UserManager<ApplicationUser> UserManager

<ul class="navbar-nav">
@if (SignInManager.IsSignedIn(User))
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-controller="Account" asp-action="Manage" title="Manage">
            Hello @User.Identity?.Name!
        </a>
    </li>
    <li class="nav-item">
        <form class="form-inline" asp-area="" asp-controller="Account" asp-action="Logout" method="post">
            <button type="submit" class="nav-link btn btn-link text-dark">Logout</button>
        </form>
    </li>
}
else
{
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-controller="Account" asp-action="Register">Register</a>
    </li>
    <li class="nav-item">
        <a class="nav-link text-dark" asp-area="" asp-controller="Account" asp-action="Login">Login</a>
    </li>
}
</ul>
```

### 5. Secure Views

```html
<!-- Views/Secure/Index.cshtml -->
@{
    ViewData["Title"] = "Secure Area";
}

<h2>@ViewData["Title"]</h2>
<p>@ViewBag.Message</p>

<div class="row">
    <div class="col-md-12">
        <h4>Available Secure Pages:</h4>
        <ul>
            <li><a asp-action="UserArea">User Area</a> (Users and Admins)</li>
            @if (User.IsInRole("Admin"))
            {
                <li><a asp-action="AdminOnly">Admin Only</a> (Admins only)</li>
                <li><a asp-action="AdminPolicy">Admin Policy</a> (Using custom policy)</li>
            }
            <li><a asp-action="Public">Public Page</a> (Everyone)</li>
        </ul>
    </div>
</div>
```

### 6. Access Denied View

```html
<!-- Views/Account/AccessDenied.cshtml -->
@{
    ViewData["Title"] = "Access Denied";
}

<header>
    <h1 class="text-danger">@ViewData["Title"]</h1>
    <p class="text-danger">You do not have access to this resource.</p>
</header>

<div class="row">
    <div class="col-md-12">
        <p>You don't have permission to access this page. This could be because:</p>
        <ul>
            <li>You need to be logged in</li>
            <li>You need specific roles or permissions</li>
            <li>The resource doesn't exist</li>
        </ul>
        
        <a asp-controller="Home" asp-action="Index" class="btn btn-primary">Go to Home</a>
        <a asp-controller="Account" asp-action="Login" class="btn btn-secondary">Login</a>
    </div>
</div>
```

## Testing the Application

### 1. Database Migration

```bash
# Create and apply migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. Run the Application

```bash
dotnet run
```

### 3. Testing Registration and Authorization

1. **Registration Test:**
   - Navigate to `/Account/Register`
   - Create a new user account
   - Verify automatic login after registration
   - Check that user gets "User" role by default

2. **Login Test:**
   - Logout and navigate to `/Account/Login`
   - Login with created credentials
   - Verify successful authentication

3. **Authorization Test:**
   - Access `/Secure/Index` (requires authentication)
   - Access `/Secure/AdminOnly` (requires Admin role - should be denied for regular users)
   - Login as admin (admin@example.com / Admin@123) and test admin-only pages

4. **Access Denied Test:**
   - Try accessing admin pages as regular user
   - Verify redirect to Access Denied page

### 4. Default Accounts

The application creates these default accounts:

- **Admin User:**
  - Email: admin@example.com
  - Password: Admin@123
  - Role: Admin

- **Regular Users:**
  - Created through registration
  - Default Role: User

### 5. Authorization Scenarios

| Page             | Anonymous | User Role | Admin Role |
| ---------------- | --------- | --------- | ---------- |
| Home             | ✅         | ✅         | ✅          |
| Privacy          | ❌         | ✅         | ✅          |
| Secure/Index     | ❌         | ✅         | ✅          |
| Secure/UserArea  | ❌         | ✅         | ✅          |
| Secure/AdminOnly | ❌         | ❌         | ✅          |
| Secure/Public    | ✅         | ✅         | ✅          |

## Summary

This tutorial covered the essential aspects of ASP.NET Core Identity for Registration and Authorization:

### What We Implemented:
- ✅ **Custom User Model** with additional properties
- ✅ **User Registration** with validation and automatic role assignment
- ✅ **Login/Logout** with security features (lockout, remember me)
- ✅ **Role-based Authorization** (Admin, User roles)
- ✅ **Policy-based Authorization** with custom policies
- ✅ **Authorization Attributes** ([Authorize], [AllowAnonymous])
- ✅ **Complete UI** with Bootstrap styling
- ✅ **Security Features** (password policies, account lockout, anti-forgery tokens)

### Key Security Features:
- Password complexity requirements
- Account lockout after failed attempts
- Anti-forgery token protection
- Role-based access control
- Secure cookie configuration
- Access denied handling

### Next Steps:
If you want to extend this application, consider adding:
- Email confirmation for registration
- Password reset functionality
- Two-factor authentication
- Claims-based authorization
- External authentication providers
- User profile management

This foundation provides a secure, scalable authentication and authorization system for ASP.NET Core applications.