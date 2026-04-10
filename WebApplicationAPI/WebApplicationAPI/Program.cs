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

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddFinancialControlServices();

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