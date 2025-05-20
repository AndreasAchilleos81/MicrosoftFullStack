using Microsoft.AspNetCore.SignalR;
using Shared.Models;
using Shared.Interfaces;

public class DataHub : Hub
{
    private readonly IGenericRepository<User> _userRepository;

    public DataHub(IGenericRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task SaveUser(User user)
    {
        await _userRepository.Add(user);
        // Optionally notify clients
    }
}