using WeatherApp.Models;

namespace WeatherApp.Services;

public class WeatherStateService
{
    public List<User> Users { get; private set; } = new List<User>();

    public Action? OnChange;

    public void SetUsers(List<User> users)
    {
        Users = users;
        NotifyStateChanged();
    }

    public void NotifyStateChanged() => OnChange?.Invoke();
}
