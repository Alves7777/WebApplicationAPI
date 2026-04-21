using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FinancialControlUI.Helpers;

public static class HttpClientExtensions
{
    public static void LogHeaders(this HttpClient httpClient, string context = "")
    {
        Console.WriteLine($"=== HttpClient Headers ({context}) ===");
        Console.WriteLine($"BaseAddress: {httpClient.BaseAddress}");

        if (httpClient.DefaultRequestHeaders.Authorization != null)
        {
            var token = httpClient.DefaultRequestHeaders.Authorization.Parameter;
            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"Authorization: Bearer {token.Substring(0, Math.Min(20, token.Length))}...");
            }
        }
        else
        {
            Console.WriteLine("Authorization: NENHUM TOKEN CONFIGURADO!");
        }
    }

    public static async Task<T?> GetAndLogAsync<T>(this HttpClient httpClient, string url, string context = "")
    {
        httpClient.LogHeaders(context);
        Console.WriteLine($"GET Request: {url}");

        var response = await httpClient.GetAsync(url);
        Console.WriteLine($"Response Status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
            return default;
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
