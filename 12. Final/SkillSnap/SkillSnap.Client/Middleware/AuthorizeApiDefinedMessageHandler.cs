using SkillSnap.Client.Services;
using System.Net.Http.Headers;

namespace SkillSnap.Client.Middleware
{
    public class AuthorizeApiDefinedMessageHandler : DelegatingHandler
    {
        private readonly AuthService _authService;
        public AuthorizeApiDefinedMessageHandler(AuthService authService)
            => _authService = authService;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Console.WriteLine("Middleware triggered...");

            var token = await _authService.GetTokenAsync();

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("DEBUG: No token found in localStorage!");
            }
            else
            {
                Console.WriteLine($"DEBUG: Token retrieved! Length: {token.Length}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
