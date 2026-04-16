using System.Net.Http.Headers;

namespace FinancialControlUI.Services;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public AuthHttpMessageHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Add token to all requests except login and register
        var isAuthEndpoint = request.RequestUri?.ToString().Contains("/auth/") ?? false;

        if (!isAuthEndpoint)
        {
            var token = await _authService.GetTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        var response = await base.SendAsync(request, cancellationToken);

        // Handle 401 Unauthorized
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _authService.LogoutAsync();
            // Navigation will be handled by the AuthorizeRoute component
        }

        return response;
    }
}
