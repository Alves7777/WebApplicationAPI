using DbUp;
using System.Reflection;
using WebApplicationAPI.Repositories;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services;
using WebApplicationAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==============================================
// MIGRATIONS AUTOMÁTICAS (DbUp)
// ==============================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("🚀 Iniciando migrations...");

// Garante que o banco existe (cria se não existir)
EnsureDatabase.For.SqlDatabase(connectionString);

// Configura e executa migrations
var upgrader = DeployChanges.To
    .SqlDatabase(connectionString)
    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
    .LogToConsole()
    .Build();

var result = upgrader.PerformUpgrade();

if (!result.Successful)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"❌ Erro ao executar migrations: {result.Error}");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Migrations executadas com sucesso!");
Console.ResetColor();

// ==============================================
// DEPENDENCY INJECTION (Design Patterns)
// ==============================================

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();

// ==============================================
// Configuração da API
// ==============================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();