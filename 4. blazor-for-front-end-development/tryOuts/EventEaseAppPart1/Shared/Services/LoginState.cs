namespace Shared.Services
{
	public class LoginState
	{
		private string Id = Guid.NewGuid().ToString();
		private readonly ApplicationStorage _storage;
		private bool? _isLoggedIn;
		private string? _userId;

		public bool? IsLoggedIn => _isLoggedIn;
		public string? UserId => _userId;

		public LoginState(ApplicationStorage storage)
		{
			_storage = storage;
			// Pull from storage once at start
			_ = InitializeFromStorage();
		}

		public async Task InitializeFromStorage()
		{
			_userId = await _storage.GetItem(_storage.UserKey)
							   ?? string.Empty;
		}

		public async Task SetLoginState(string userId, bool isLoggedIn)
		{
			_userId = userId;
			_isLoggedIn = isLoggedIn;
			await _storage.AddItem(_storage.UserKey, userId);
		}

		public async Task Clear()
		{
			_userId = null;
			_isLoggedIn = null;
			await _storage.RemoveItem(_storage.UserKey);
		}
	}
}
