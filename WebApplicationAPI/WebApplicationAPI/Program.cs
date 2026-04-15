using DbUp;
using System.Reflection;
using WebApplicationAPI.Repositories;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services;
using WebApplicationAPI.Services.Interfaces;
using WebApplicationAPI.DependencyInjection;
using FluentValidation;
using MediatR;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar para aceitar conexões externas (celular, tablets, etc)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5296); // HTTP
    options.ListenAnyIP(7296, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS
    });
});

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveSecretaSuperSegura123!@#MinimoDe32Caracteres";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "WebApplicationAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "WebApplicationAPI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // Configurar respostas personalizadas para erros de autenticação
    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            // Impedir a resposta padrão
            context.HandleResponse();

            // Definir status code
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            // Determinar mensagem baseada no erro
            var message = "Não autorizado. Token de autenticação ausente ou inválido.";

            if (context.Error == "invalid_token")
            {
                message = "Token inválido.";
            }
            else if (context.ErrorDescription?.Contains("expired") == true)
            {
                message = "Token expirado. Faça login novamente.";
            }
            else if (string.IsNullOrEmpty(context.Request.Headers["Authorization"]))
            {
                message = "Token de autenticação não fornecido.";
            }

            // Retornar JSON
            var result = System.Text.Json.JsonSerializer.Serialize(new
            {
                message = message,
                statusCode = 401
            });

            return context.Response.WriteAsync(result);
        },
        OnAuthenticationFailed = context =>
        {
            // Log do erro (opcional)
            Console.WriteLine($"Falha na autenticação: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Adicionar HttpContextAccessor e UserContext para multi-tenancy
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<WebApplicationAPI.Helpers.UserContext>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddFinancialControlServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Comentar UseHttpsRedirection para permitir HTTP no celular
// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();