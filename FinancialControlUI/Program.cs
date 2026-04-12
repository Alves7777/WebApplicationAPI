using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FinancialControlUI;
using FinancialControlUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Lê a URL da API do arquivo de configuração
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5296/api/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<CreditCardService>();
builder.Services.AddScoped<MonthlyFinancialService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SummaryService>();

await builder.Build().RunAsync();
