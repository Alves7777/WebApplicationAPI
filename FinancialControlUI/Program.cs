using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FinancialControlUI;
using FinancialControlUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Lê a URL da API do arquivo de configuração
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://192.168.0.178:7296/api/";

// Configurar HttpClient com interceptor de autenticação
builder.Services.AddScoped<AuthService>();

// Adicionar HttpClient com base address
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
    return httpClient;
});

builder.Services.AddScoped<CreditCardService>();
builder.Services.AddScoped<MonthlyFinancialService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SummaryService>();


var host = builder.Build();

// Initialize AuthService to set token in HttpClient
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InitializeAsync();

await host.RunAsync();
