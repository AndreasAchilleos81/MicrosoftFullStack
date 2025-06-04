using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;
using Shared.Extentions;
using Shared.Result;
using Shared.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Repository;

public class DataHub : Hub
{
	private readonly IGenericRepository<User> _userRepository;
	private readonly IGenericRepository<Registration> _registrationRepository;
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SessionManagement _sessionManagement;
	private readonly ApplicationStorage _applicationStorage;

	public DataHub(IGenericRepository<User> userRepository,
				   IGenericRepository<Registration> registrationRepository,
				   UserManager<IdentityUser> userManager,
				   SignInManager<IdentityUser> signInManager,
				   SessionManagement sessionManagement,
				   ApplicationStorage applicationStorage)
	{
		_userRepository = userRepository;
		_registrationRepository = registrationRepository;
		_userManager = userManager;
		_sessionManagement = sessionManagement;
		_applicationStorage = applicationStorage;
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

		// Add session has started
		await _sessionManagement.SessionStart(user.Id);

		return new LoginResult { Succeeded = true };
	}

	public async Task LogoutUser(string userId)
	{
		await _sessionManagement.SessionStop(userId);
	}

	public async Task<string> GetUserId(string email)
	{
		var userRepo = (UserRepository)_userRepository;
		var user = await userRepo.GetUserByEmail(email);
		if (user == null)
			return null;
		return user.Id;
	}

	private RegistrationResult ConvertIdentityResult(IdentityResult result)
	{
		var registrationResult = new RegistrationResult();
		registrationResult.Succeeded = result.Succeeded;
		registrationResult.Errors = result.Errors.Select(e => e.Description);
		return registrationResult;
	}
}