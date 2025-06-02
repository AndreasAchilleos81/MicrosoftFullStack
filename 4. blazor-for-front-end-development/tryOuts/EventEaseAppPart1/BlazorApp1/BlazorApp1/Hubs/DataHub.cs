using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;
using Shared.Extentions;
using Shared.Result;
using Microsoft.AspNetCore.Identity;

public class DataHub : Hub
{
	private readonly IGenericRepository<User> _userRepository;
	private readonly IGenericRepository<Registration> _registrationRepository;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;

	public DataHub(IGenericRepository<User> userRepository,
				   IGenericRepository<Registration> registrationRepository,
				   UserManager<IdentityUser> userManager,
				   SignInManager<IdentityUser> signInManager)
	{
		_userRepository = userRepository;
		_registrationRepository = registrationRepository;
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public async Task<RegistrationResult> SaveUser(User user)
	{
		var identityUser = new IdentityUser
		{
			UserName = user.Email,
			Email = user.Email
		};

		var result = await _userManager.CreateAsync(identityUser, user.Password);

		if (!result.Succeeded)
		{
			return ConvertIdentityResult(result);
		}

		await _userRepository.Add(user);
		await _registrationRepository.Add(user.CreateRegistration());

		return ConvertIdentityResult(result);
	}


	public async Task<LoginResult> LoginUser(LoginModel loginModel)
	{
		var user = await _userManager.FindByEmailAsync(loginModel.Email);
		if (user == null)
			return new LoginResult { Succeeded = false, Errors = new[] { "User not found." } };

		var passwordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
		if (!passwordValid)
			return new LoginResult { Succeeded = false, Errors = new[] { "Invalid credentials." } };

		return new LoginResult { Succeeded = true };
	}

	private RegistrationResult ConvertIdentityResult(IdentityResult result)
	{
		var registrationResult = new RegistrationResult();
		registrationResult.Succeeded = result.Succeeded;
		registrationResult.Errors = result.Errors.Select(e => e.Description);
		return registrationResult;
	}
}