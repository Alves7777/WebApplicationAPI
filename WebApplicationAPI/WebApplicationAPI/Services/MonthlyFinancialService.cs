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

        public async Task<MonthlyFinancialResponse> CreateAsync(int userId, CreateMonthlyFinancialRequest request)
        {
            var existing = await _repository.GetByYearAndMonthAsync(request.Year, request.Month);
            if (existing != null && existing.UserId == userId)
            {
                throw new InvalidOperationException($"Já existe um registro para {request.Month}/{request.Year}");
            }

            var entity = new MonthlyFinancialControl
            {
                UserId = userId, // ? Vincula ao usuário logado
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

        public async Task<MonthlyFinancialResponse> UpdateAsync(int id, int userId, UpdateMonthlyFinancialRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new InvalidOperationException("Registro não encontrado");
            }

            // ? Validar ownership
            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar este registro");
            }

            var duplicate = await _repository.GetByYearAndMonthAsync(request.Year, request.Month);
            if (duplicate != null && duplicate.Id != id && duplicate.UserId == userId)
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

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return false;
            }

            // ? Validar ownership
            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para deletar este registro");
            }

            return await _repository.DeleteAsync(id);
        }

        public async Task<MonthlyFinancialResponse> GetByIdAsync(int id, int userId)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return null;
            }

            // ? Validar ownership
            if (entity.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para acessar este registro");
            }

            return await MapToResponse(entity);
        }

        public async Task<List<MonthlyFinancialResponse>> GetAllAsync(int userId)
        {
            // ? Busca apenas do usuário logado
            var entities = await _repository.GetByUserIdAsync(userId);
            var responses = new List<MonthlyFinancialResponse>();

            foreach (var entity in entities)
            {
                responses.Add(await MapToResponse(entity));
            }

            return responses.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).ToList();
        }

        public async Task<MonthlyFinancialResponse> GetByYearAndMonthAsync(int userId, int year, int month)
        {
            var entity = await _repository.GetByYearAndMonthAsync(year, month);
            if (entity == null)
            {
                return null;
            }

            // ? Validar ownership
            if (entity.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para acessar este registro");
            }

            return await MapToResponse(entity);
        }

        public async Task<List<MonthlyFinancialResponse>> GetByYearAsync(int userId, int year)
        {
            // ? Busca apenas do usuário logado
            var entities = await _repository.GetByUserIdAsync(userId);
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
            var salaryTotal = entity.Money + entity.RV + entity.Debit + entity.Others;
            var expensesTotal = await _repository.GetExpensesTotalByYearAndMonthAsync(entity.Year, entity.Month);
            var balance = salaryTotal - expensesTotal;
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
