using Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services
{
	public class ProfilesService
	{
		private readonly HttpClient _httpClient;
		private const string getProjectsEndpoint = "api/PortfolioUsers/GetPortfolioUser";

		public ProfilesService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<PortfolioUser>> GetProfilesAsync()
		{
			List<PortfolioUser> portfolioUsers = new();
			if (_httpClient != null)
			{
				try
				{
					portfolioUsers = await _httpClient.GetFromJsonAsAsyncEnumerable<PortfolioUser>(getProjectsEndpoint).ToListAsync();
				}
				catch { }
			}

			return portfolioUsers;
		}
	}
}
