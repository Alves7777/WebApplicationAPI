using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using FinancialControlUI.Models;

namespace FinancialControlUI.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _jsRuntime;
    private const string TOKEN_KEY = "authToken";
    private const string EXPIRES_KEY = "tokenExpires";
    private const string USER_ID_KEY = "userId";
    private const string USER_NAME_KEY = "userName";
    private const string USER_EMAIL_KEY = "userEmail";

    public AuthService(HttpClient http, IJSRuntime jsRuntime)
    {
        _http = http;
        _jsRuntime = jsRuntime;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        Console.WriteLine($"=== LoginAsync - Enviando requisição ===");
        Console.WriteLine($"Email: {request.Email}");

        var response = await _http.PostAsJsonAsync("auth/login", request);

        Console.WriteLine($"Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erro no login: {error}");
            throw new HttpRequestException($"Login failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

        if (result != null)
        {
            Console.WriteLine($"=== LoginAsync - Resposta recebida ===");
            Console.WriteLine($"UserId: {result.UserId}");
            Console.WriteLine($"Name: {result.Name}");
            Console.WriteLine($"Email: {result.Email}");
            Console.WriteLine($"Token: {result.Token.Substring(0, 20)}...");
            Console.WriteLine($"ExpiresAt: {result.ExpiresAt}");
        }
        else
        {
            Console.WriteLine("?? Resultado do login é NULL!");
        }

        return result;
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var response = await _http.PostAsJsonAsync("auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Registration failed: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return result;
    }

    public async Task SaveTokenAsync(AuthResponse authResponse)
    {
        Console.WriteLine($"=== Salvando Token ===");
        Console.WriteLine($"Token: {authResponse.Token.Substring(0, 20)}...");
        Console.WriteLine($"User: {authResponse.Name} ({authResponse.Email})");
        Console.WriteLine($"ExpiresAt recebido do backend: {authResponse.ExpiresAt}");
        Console.WriteLine($"ExpiresAt.Kind: {authResponse.ExpiresAt.Kind}");

        // Garantir que a data seja salva em UTC
        var expiresAtUtc = authResponse.ExpiresAt.Kind == DateTimeKind.Utc 
            ? authResponse.ExpiresAt 
            : authResponse.ExpiresAt.ToUniversalTime();

        var expiresStr = expiresAtUtc.ToString("o");
        Console.WriteLine($"ExpiresAt salvo (UTC, ISO 8601): {expiresStr}");

        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TOKEN_KEY, authResponse.Token);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", EXPIRES_KEY, expiresStr);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_ID_KEY, authResponse.UserId.ToString());
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_NAME_KEY, authResponse.Name);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", USER_EMAIL_KEY, authResponse.Email);

        // Set the default authorization header
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.Token);
        Console.WriteLine("Token configurado no HttpClient");
    }

    public async Task<string?> GetTokenAsync()
    {
        Console.WriteLine("=== GetTokenAsync ===");
        var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TOKEN_KEY);

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Token não encontrado no localStorage");
        }
        else
        {
            Console.WriteLine($"Token recuperado: {token.Substring(0, Math.Min(20, token.Length))}...");
        }

        return token;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        if (string.IsNullOrEmpty(token))
            return false;

        return !await IsTokenExpiredAsync();
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var expiresStr = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", EXPIRES_KEY);

        if (string.IsNullOrEmpty(expiresStr))
        {
            Console.WriteLine("Token expirado: nenhuma data de expiração encontrada");
            return true;
        }

        // Usar DateTime.Parse com DateTimeStyles.RoundtripKind para preservar UTC
        if (DateTime.TryParse(expiresStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime expiresAt))
        {
            // Garantir que está em UTC
            if (expiresAt.Kind != DateTimeKind.Utc)
            {
                expiresAt = DateTime.SpecifyKind(expiresAt, DateTimeKind.Utc);
            }

            var now = DateTime.UtcNow;
            var isExpired = now >= expiresAt;

            Console.WriteLine($"Verificação de expiração:");
            Console.WriteLine($"  Agora (UTC): {now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Expira em (UTC): {expiresAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Está expirado? {isExpired}");
            Console.WriteLine($"  Tempo restante: {(expiresAt - now).TotalMinutes:F2} minutos");

            return isExpired;
        }

        Console.WriteLine($"Token expirado: não foi possível parsear a data '{expiresStr}'");
        return true;
    }

    public async Task LogoutAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TOKEN_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", EXPIRES_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_ID_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_NAME_KEY);
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", USER_EMAIL_KEY);

        // Clear the authorization header
        _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<int> GetUserIdAsync()
    {
        var userIdStr = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", USER_ID_KEY);
        return int.TryParse(userIdStr, out int userId) ? userId : 0;
    }

    public async Task<string?> GetUserNameAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", USER_NAME_KEY);
    }

    public async Task<string?> GetUserEmailAsync()
    {
        return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", USER_EMAIL_KEY);
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine("=== Inicializando AuthService ===");
        var token = await GetTokenAsync();

        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"Token encontrado: {token.Substring(0, Math.Min(20, token.Length))}...");

            if (!await IsTokenExpiredAsync())
            {
                Console.WriteLine("Token válido - configurando HttpClient");
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("Token expirado - limpando");
                await LogoutAsync();
            }
        }
        else
        {
            Console.WriteLine("Nenhum token encontrado");
        }
    }
}
