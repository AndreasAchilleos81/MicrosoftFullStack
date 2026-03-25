using Shared.Models;
using Shared.Models.DTO;
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

	public async Task<Skills> GetSkillsAsync()
	{
        Skills skills = new();
		if (_httpClient == null)
		{
			return skills;
		}
		try
		{
			skills = await _httpClient.GetFromJsonAsync<Skills>(getSkillsEndpoint);
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