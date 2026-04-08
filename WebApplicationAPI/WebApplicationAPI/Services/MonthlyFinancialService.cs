using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Enums;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class MonthlyFinancialService : IMonthlyFinancialService
    {
        private readonly IMonthlyFinancialRepository _repository;

        public MonthlyFinancialService(IMonthlyFinancialRepository repository)
        {
            _repository = repository;
        }

        public async Task<MonthlyFinancialResponse> CreateAsync(CreateMonthlyFinancialRequest request)
        {
            // Validar duplicidade
            var existing = await _repository.GetByYearAndMonthAsync(request.Year, request.Month);
            if (existing != null)
            {
                throw new InvalidOperationException($"Já existe um registro para {request.Month}/{request.Year}");
            }

            var entity = new MonthlyFinancialControl
            {
                Year = request.Year,
                Month = request.Month,
                Money = request.Money,
                RV = request.RV,
                Debit = request.Debit,
                Others = request.Others,
                Reserve = request.Reserve,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var id = await _repository.CreateAsync(entity);
            entity.Id = id;

            return await MapToResponse(entity);
        }

        public async Task<MonthlyFinancialResponse> UpdateAsync(int id, UpdateMonthlyFinancialRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new InvalidOperationException("Registro não encontrado");
            }

            // Validar duplicidade (exceto o próprio registro)
            var duplicate = await _repository.GetByYearAndMonthAsync(request.Year, request.Month);
            if (duplicate != null && duplicate.Id != id)
            {
                throw new InvalidOperationException($"Já existe outro registro para {request.Month}/{request.Year}");
            }

            existing.Year = request.Year;
            existing.Month = request.Month;
            existing.Money = request.Money;
            existing.RV = request.RV;
            existing.Debit = request.Debit;
            existing.Others = request.Others;
            existing.Reserve = request.Reserve;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);

            return await MapToResponse(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<MonthlyFinancialResponse> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            return await MapToResponse(entity);
        }

        public async Task<List<MonthlyFinancialResponse>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            var responses = new List<MonthlyFinancialResponse>();

            foreach (var entity in entities)
            {
                responses.Add(await MapToResponse(entity));
            }

            return responses.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToList();
        }

        public async Task<MonthlyFinancialResponse> GetByYearAndMonthAsync(int year, int month)
        {
            var entity = await _repository.GetByYearAndMonthAsync(year, month);
            if (entity == null)
            {
                return null;
            }

            return await MapToResponse(entity);
        }

        public async Task<List<MonthlyFinancialResponse>> GetByYearAsync(int year)
        {
            var entities = await _repository.GetAllAsync();
            var filtered = entities.Where(x => x.Year == year).ToList();
            var responses = new List<MonthlyFinancialResponse>();

            foreach (var entity in filtered)
            {
                responses.Add(await MapToResponse(entity));
            }

            return responses.OrderBy(x => x.Month).ToList();
        }

        private async Task<MonthlyFinancialResponse> MapToResponse(MonthlyFinancialControl entity)
        {
            // Calcular SalaryTotal
            var salaryTotal = entity.Money + entity.RV + entity.Debit + entity.Others;

            // Buscar ExpensesTotal
            var expensesTotal = await _repository.GetExpensesTotalByYearAndMonthAsync(entity.Year, entity.Month);

            // Calcular Balance
            var balance = salaryTotal - expensesTotal;

            // Calcular CanSpend
            var canSpend = balance - entity.Reserve;

            return new MonthlyFinancialResponse
            {
                Id = entity.Id,
                Year = entity.Year,
                Month = entity.Month,
                MonthName = GetMonthName(entity.Month),
                Money = entity.Money,
                RV = entity.RV,
                Debit = entity.Debit,
                Others = entity.Others,
                Reserve = entity.Reserve,
                SalaryTotal = salaryTotal,
                ExpensesTotal = expensesTotal,
                Balance = balance,
                CanSpend = canSpend,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        private string GetMonthName(int month)
        {
            if (Enum.IsDefined(typeof(MonthEnum), month))
            {
                return ((MonthEnum)month).ToString();
            }
            return month.ToString();
        }
    }
}
