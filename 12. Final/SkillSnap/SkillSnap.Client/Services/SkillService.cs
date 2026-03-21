using Shared.Models;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class SkillService
{
	private readonly HttpClient _httpClient;

	private const string getSkillsEndpoint = "api/Skills/GetSkills";
	private const string addSkillEndpoint = "api/Skills/AddSkill";

	public SkillService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<List<Skill>> GetSkillsAsync()
	{
		List<Skill> skills = new List<Skill>();
		if (_httpClient == null)
		{
			return skills;
		}
		try
		{
			skills = await _httpClient.GetFromJsonAsAsyncEnumerable<Skill>(getSkillsEndpoint).ToListAsync();
		}
		catch (Exception ex) { }

		return skills;
	}

	public async Task<HttpResponseMessage> AddProject(Skill skill)
	{
		var response = await _httpClient.PostAsJsonAsync(addSkillEndpoint, skill);
		return response;
	}
}