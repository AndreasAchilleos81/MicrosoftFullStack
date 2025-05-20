using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;
using Shared.Extentions;

public class DataHub : Hub
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IGenericRepository<Registration> _registrationRepository;

    public DataHub(IGenericRepository<User> userRepository,
				   IGenericRepository<Registration> registrationRepository)
    {
        _userRepository = userRepository;
		_registrationRepository = registrationRepository;
	}

    public async Task SaveUser(User user)
    {
        await _userRepository.Add(user);
        await _registrationRepository.Add(user.CreateRegistration());
	}
}