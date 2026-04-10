using Microsoft.Extensions.DependencyInjection;
using WebApplicationAPI.Repositories;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services;
using WebApplicationAPI.Services.Interfaces;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace WebApplicationAPI.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFinancialControlServices(this IServiceCollection services)
        {
            services.AddScoped<IExpenseRepository, ExpenseRepository>();
            services.AddScoped<IFinancialCalculatorService, FinancialCalculatorService>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IMonthlyFinancialRepository, MonthlyFinancialRepository>();
            services.AddScoped<IMonthlyFinancialService, MonthlyFinancialService>();

            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICreditCardService, CreditCardService>();

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            return services;
        }
    }
}