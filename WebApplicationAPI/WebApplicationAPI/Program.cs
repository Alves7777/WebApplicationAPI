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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
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
app.UseAuthorization();
app.MapControllers();

app.Run();