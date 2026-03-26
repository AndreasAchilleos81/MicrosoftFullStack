using Microsoft.JSInterop;

namespace SkillSnap.Client.Services;

public class UserSessionService
{
    private readonly IJSRuntime _jsRuntime;
    private const string userIdKey = "userId";
    private const string roleKey = "role";
    private const string tokenKey = "token";
    private const string projectkey = "projectId";

    public UserSessionService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task SaveSession(string userId, string role, string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", userIdKey, userId);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", roleKey, role);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", tokenKey, token);
    }

    public async Task<(string, string)> GetSession()
    {
        var userid = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", userIdKey);
        var userrole = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", roleKey); 
        return await Task.FromResult((userid, userrole));
    }

    public async Task SetCurrentProjectId(int projectId)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", projectkey, projectId.ToString());
    }

    public async Task RemoveCurrentProjectId(int projectId)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", projectkey);
    }
}
