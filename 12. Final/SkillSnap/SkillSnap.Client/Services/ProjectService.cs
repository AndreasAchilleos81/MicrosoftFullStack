using Shared.Models;
using Shared.Models.DTO;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public class ProjectService
{
	private readonly HttpClient _httpClient;
	private const string _getProjectsEndpoint = "api/Projects/GetProjects";
	private const string _addProjectEndpoint = "api/Projects/AddProject";
	private const string _getProjectById = "api/Projects/GetProject?id={0}";
	private const string _updateProjectById = "api/Projects/UpdateProject";
	private const string _updatePortfolioUsers = "/api/PortfolioUsers/UpdatePortfolioUsers";


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
			projects =  await _httpClient.GetFromJsonAsync<Projects>(_getProjectsEndpoint);
		}
		catch(Exception ex) 
		{
			Console.WriteLine($"Error fetching projects: {ex.Message}");
		}

		return projects;
	}

	public async Task<ProjectDto> GetProjectAsync(int id)
	{
		ProjectDto project = new();
		if (_httpClient == null)
		{
			return project!;
		}
		try
		{
			//project = await _httpClient.GetFromJsonAsync<Project>($"api/Projects/GetProject?id={id}");
			project = await _httpClient.GetFromJsonAsync<ProjectDto>(string.Format(_getProjectById, id));
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching project with ID {id}: {ex.Message}");
		}
		return project;
    }

    public async Task<HttpResponseMessage> AddProject(Project project)
	{
		var response = await _httpClient.PostAsJsonAsync(_addProjectEndpoint, project);
		return response;
	}

	public async Task<HttpResponseMessage> UpdateProject(ProjectDto project)
	{
        var response = await _httpClient.PutAsJsonAsync(_updateProjectById, project);
		return response;
    }


	public async Task<HttpResponseMessage> UpdatePortfolioUsers(IEnumerable<PortfolioUser> users) 
	{
		return await _httpClient.PostAsJsonAsync(_updatePortfolioUsers, users);		
	}

    public async Task<HttpResponseMessage> DeleteProject(int id)
	{
		var response = await _httpClient.DeleteAsync($"api/Projects/DeleteProject?id={id}");
		return response;
    }

}
