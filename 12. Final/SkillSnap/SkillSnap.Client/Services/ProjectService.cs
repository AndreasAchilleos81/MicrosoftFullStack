using Shared.Models;
using Shared.Models.DTO;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class ProjectService
{
	private readonly HttpClient _httpClient;
	private const string getProjectsEndpoint = "api/Projects/GetProjects";
	private const string addProjectEndpoint = "api/Projects/AddProject";

	public ProjectService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<Projects> GetProjectsAsync()
	{
        Projects projects = new();

		if (_httpClient == null)
		{
			return projects;
		}
		try
		{
			projects =  await _httpClient.GetFromJsonAsync<Projects>(getProjectsEndpoint);
		}
		catch(Exception ex) 
		{
			Console.WriteLine($"Error fetching projects: {ex.Message}");
		}

		return projects;
	}

	public async Task<HttpResponseMessage> AddProject(Project project)
	{
		var response = await _httpClient.PostAsJsonAsync(addProjectEndpoint, project);
		return response;
	}
}
