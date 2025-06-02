using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;
using Shared.Extentions;
using Shared.Result;
using Shared.Services;
using Microsoft.AspNetCore.Identity;

public class DataHub : Hub
{
	private readonly IGenericRepository<User> _userRepository;
	private readonly IGenericRepository<Registration> _registrationRepository;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SessionManagement _sessionManagement;

	public DataHub(IGenericRepository<User> userRepository,
				   IGenericRepository<Registration> registrationRepository,
				   UserManager<IdentityUser> userManager,
				   SignInManager<IdentityUser> signInManager,
				   SessionManagement sessionManagement)
	{
		_userRepository = userRepository;
		_registrationRepository = registrationRepository;
		_userManager = userManager;
		_sessionManagement = sessionManagement;
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

		// syncing on registration Ids in AspNetUser and Registration Table
		user.Id = _userManager.Users.First(u => u.Email == user.Email).Id;
		
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


		await _sessionManagement.SessionStart(user.Id);
		
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