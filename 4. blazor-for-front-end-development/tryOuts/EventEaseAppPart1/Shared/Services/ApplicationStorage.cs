using Microsoft.JSInterop;
namespace Shared.Services
{
	public class ApplicationStorage
	{
		private readonly IJSRuntime _js;
		public string UserKey { get; } = "userId";
		public ApplicationStorage(IJSRuntime JS)
		{
				_js = JS;
		}

		public async Task<string> GetItem(string key)
		{
			return await _js.InvokeAsync<string>("localStorage.getItem", key);
		}

		public async Task AddItem(string key, string value)
		{
			await _js.InvokeVoidAsync("localStorage.setItem", key, value);
		}

		public async Task RemoveItem(string key)
		{
			await _js.InvokeVoidAsync("localStorage.removeItem", key);

		}
	}
}
