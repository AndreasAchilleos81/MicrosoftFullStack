using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;
using Shared.Extentions;
using Shared.Result;
using Shared.Services;
using Microsoft.AspNetCore.Identity;
using Shared.Repository;
using System.Runtime.CompilerServices;

public class DataHub : Hub
{
	private readonly IGenericRepository<User> _userRepository;
	private readonly IGenericRepository<Registration> _registrationRepository;
	private readonly IGenericRepository<Session> _sessionRepository;
	private readonly IGenericRepository<EventCard> _eventCardRepository;
	private readonly IGenericRepository<Attendance> _attendanceRepository;

	private readonly UserManager<IdentityUser> _userManager;
	private readonly SessionManagement _sessionManagement;
	private readonly ApplicationStorage _applicationStorage;


	public DataHub(IGenericRepository<User> userRepository,
				   IGenericRepository<Registration> registrationRepository,
				   IGenericRepository<Session> sessionRepository,
				   IGenericRepository<EventCard> eventCardRepository,
					IGenericRepository<Attendance> attendanceRepository,
				   UserManager<IdentityUser> userManager,
				   SignInManager<IdentityUser> signInManager,
				   SessionManagement sessionManagement,
				   ApplicationStorage applicationStorage)
	{
		_userRepository = userRepository;
		_registrationRepository = registrationRepository;
		_sessionRepository = sessionRepository;
		_attendanceRepository = attendanceRepository;
		_userManager = userManager;
		_sessionManagement = sessionManagement;
		_applicationStorage = applicationStorage;
		_eventCardRepository = eventCardRepository;
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

	public async Task<bool> IsAdmin(string userId)
	{
		if (string.IsNullOrEmpty(userId)) return false;
		var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
		var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
		return isInRole;
	}

	public async Task<bool> IsSessionActive(string userId)
	{
		var sessionRepo = (SessionRepository)_sessionRepository;
		return await sessionRepo.IsSessionActive(userId);
	}

	public async Task UpdateSessionStatus(string userId, bool isLoggedIn)
	{
		if (string.IsNullOrEmpty(userId)) return;
		await Clients.Caller.SendAsync("SessionStatusChanged", isLoggedIn);
	}

	public async Task<EventCard> GetCard(string cardId)
	{
		return await _eventCardRepository.GetById(cardId);
	}

	public async Task<bool> AddCard(EventCard card)
	{
		if (card == null) return false;
		return await _eventCardRepository.Add(card);
	}

	public async Task<bool> UpdateEventCard(EventCard card)
	{
		return await _eventCardRepository.Update(card);
	}

	public async Task<bool> DeleteCard(EventCard card)
	{
		return await _eventCardRepository.Delete(card);
	}

	public async Task<bool> InterestedInEvent(string eventId, string userId)
	{
		Attendance attendance = CreateEvent(eventId, userId);
		attendance.Attended = AttendanceStatus.Interested;

		return await CheckToAddOrUpdate(eventId, userId, attendance);
	}

	public async Task<bool> GoingToEvent(string eventId, string userId)
	{
		Attendance attendance = CreateEvent(eventId, userId);
		attendance.Attended = AttendanceStatus.Going;

		return await CheckToAddOrUpdate(eventId, userId, attendance);
	}

	public async Task<IEnumerable<Attendance>> Attendances(string userId)
	{
		if (string.IsNullOrEmpty(userId)) return Enumerable.Empty<Attendance>();
		var repo = (AttendanceRepository)_attendanceRepository;
		return await repo.GetAttendances(userId);
	}

	public async Task<IEnumerable<EventCard>> EventCards(IEnumerable<string> eventIds)
	{
		var repo = (EventCardRepository)_eventCardRepository;	
		return await repo.GetEvents(eventIds);
	}

	private async Task<bool> CheckToAddOrUpdate(string eventId, string userId, Attendance attendance)
	{
		var repo = (AttendanceRepository)_attendanceRepository;
		var attendanceInDb = await repo.GetAttendance(eventId, userId);
		if (attendanceInDb == null)
		{
			return await _attendanceRepository.Add(attendance);
		}
		else
		{
			return await _attendanceRepository.Update(attendance);
		}
	}

	private Attendance CreateEvent(string eventId, string userId)
	{
		return new Attendance
		{
			EventId = eventId,
			UserId = userId,
			Attended = AttendanceStatus.Interested,
		};
	}

	private RegistrationResult ConvertIdentityResult(IdentityResult result)
	{
		var registrationResult = new RegistrationResult();
		registrationResult.Succeeded = result.Succeeded;
		registrationResult.Errors = result.Errors.Select(e => e.Description);
		return registrationResult;
	}
}