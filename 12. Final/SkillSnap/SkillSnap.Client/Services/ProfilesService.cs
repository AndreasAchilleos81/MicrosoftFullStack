using Shared.Models.DTO;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class ProfilesService
{
	private readonly HttpClient _httpClient;
	private const string getProjectsEndpoint = "api/PortfolioUsers/GetPortfolioUser";

	public ProfilesService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Profiles> GetProfilesAsync()
	{
            Profiles portfolioUsers = new();
		if (_httpClient != null)
		{
			try
			{
				portfolioUsers = await _httpClient.GetFromJsonAsync<Profiles>(getProjectsEndpoint);
			}
			catch(Exception ex) 
			{
				Console.WriteLine(ex.Message);
			}
		}

		return portfolioUsers;
	}
}
