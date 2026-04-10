using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class CreditCardService : ICreditCardService
    {
        private readonly ICreditCardRepository _repository;

        public CreditCardService(ICreditCardRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreditCardResponse> CreateAsync(CreateCreditCardRequest request)
        {
            var creditCard = new CreditCard
            {
                Name = request.Name,
                Brand = request.Brand,
                CardLimit = request.CardLimit,
                ClosingDay = request.ClosingDay,
                DueDay = request.DueDay,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var id = await _repository.CreateAsync(creditCard);
            creditCard.Id = id;

            return MapToResponse(creditCard);
        }

        public async Task<CreditCardResponse> UpdateAsync(int id, UpdateCreditCardRequest request)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            existing.Name = request.Name;
            existing.Brand = request.Brand;
            existing.CardLimit = request.CardLimit;
            existing.ClosingDay = request.ClosingDay;
            existing.DueDay = request.DueDay;
            existing.IsActive = request.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return MapToResponse(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<CreditCardResponse> GetByIdAsync(int id)
        {
            var creditCard = await _repository.GetByIdAsync(id);
            return creditCard != null ? MapToResponse(creditCard) : null;
        }

        public async Task<List<CreditCardResponse>> GetAllAsync()
        {
            var creditCards = await _repository.GetAllAsync();
            return creditCards.Select(MapToResponse).ToList();
        }

        public async Task<CreditCardExpenseResponse> CreateExpenseAsync(int creditCardId, CreateCreditCardExpenseRequest request)
        {
            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var expense = new CreditCardExpense
            {
                CreditCardId = creditCardId,
                PurchaseDate = request.PurchaseDate,
                Description = request.Description,
                Amount = request.Amount,
                Category = request.Category ?? CategorizeExpense(request.Description),
                Month = request.PurchaseDate.Month,
                Year = request.PurchaseDate.Year,
                Status = request.Status,
                CreatedAt = DateTime.UtcNow
            };

            var id = await _repository.CreateExpenseAsync(expense);
            expense.Id = id;

            return MapToExpenseResponse(expense);
        }

        public async Task<CreditCardExpenseResponse> UpdateExpenseAsync(int expenseId, UpdateCreditCardExpenseRequest request)
        {
            var existing = await _repository.GetExpenseByIdAsync(expenseId);
            if (existing == null)
            {
                throw new InvalidOperationException("Despesa não encontrada");
            }

            existing.PurchaseDate = request.PurchaseDate;
            existing.Description = request.Description;
            existing.Amount = request.Amount;
            existing.Category = request.Category;
            existing.Status = request.Status;
            existing.Month = request.PurchaseDate.Month;
            existing.Year = request.PurchaseDate.Year;

            await _repository.UpdateExpenseAsync(existing);
            return MapToExpenseResponse(existing);
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            return await _repository.DeleteExpenseAsync(expenseId);
        }

        public async Task<CreditCardExpenseResponse> GetExpenseByIdAsync(int expenseId)
        {
            var expense = await _repository.GetExpenseByIdAsync(expenseId);
            return expense != null ? MapToExpenseResponse(expense) : null;
        }

        public async Task<List<CreditCardExpenseResponse>> GetExpensesByCardAsync(int creditCardId, int? month = null, int? year = null, string? category = null)
        {
            var expenses = await _repository.GetExpensesByCardAsync(creditCardId, month, year, category);
            return expenses.Select(MapToExpenseResponse).ToList();
        }

        public async Task<CsvImportResult> ImportCsvAsync(int creditCardId, Stream fileStream)
        {
            var result = new CsvImportResult();
            var errors = new List<string>();
            var categoriesCount = new Dictionary<string, int>();

            try
            {
                var card = await _repository.GetByIdAsync(creditCardId);
                if (card == null)
                {
                    throw new InvalidOperationException("Cartão não encontrado");
                }

                using var reader = new StreamReader(fileStream);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                    HeaderValidated = null
                };

                using var csv = new CsvReader(reader, config);
                var records = csv.GetRecords<CsvExpenseRecord>().ToList();
                result.TotalRecords = records.Count;

                foreach (var record in records)
                {
                    try
                    {
                        if (!DateTime.TryParse(record.Date, out var purchaseDate))
                        {
                            errors.Add($"Data inválida: {record.Date} - {record.Title}");
                            result.ErrorCount++;
                            continue;
                        }

                        if (!decimal.TryParse(record.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                        {
                            errors.Add($"Valor inválido: {record.Amount} - {record.Title}");
                            result.ErrorCount++;
                            continue;
                        }

                        if (amount < 0)
                        {
                            continue;
                        }

                        // ? VERIFICAR DUPLICIDADE
                        var exists = await _repository.ExpenseExistsAsync(
                            creditCardId,
                            purchaseDate,
                            record.Title,
                            amount
                        );

                        if (exists)
                        {
                            // Já existe, pular e contar como duplicado
                            result.DuplicateCount++;
                            errors.Add($"Duplicado (ignorado): {record.Title} - {amount:C}");
                            continue;
                        }

                        var category = CategorizeExpense(record.Title);

                        if (!categoriesCount.ContainsKey(category))
                        {
                            categoriesCount[category] = 0;
                        }
                        categoriesCount[category]++;

                        var expense = new CreditCardExpense
                        {
                            CreditCardId = creditCardId,
                            PurchaseDate = purchaseDate,
                            Description = record.Title,
                            Amount = amount,
                            Category = category,
                            Month = purchaseDate.Month,
                            Year = purchaseDate.Year,
                            Status = "PAGO",
                            CreatedAt = DateTime.UtcNow
                        };

                        await _repository.CreateExpenseAsync(expense);
                        result.ImportedRecords++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Erro: {record.Title} - {ex.Message}");
                        result.ErrorCount++;
                    }
                }

                result.Errors = errors;
                result.CategoriesCount = categoriesCount;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao importar CSV: {ex.Message}");
            }

            return result;
        }

        public async Task<CreditCardStatementResponse> GetStatementAsync(int creditCardId, int month, int year)
        {
            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var expenses = await _repository.GetExpensesByCardAsync(creditCardId, month, year);

            return new CreditCardStatementResponse
            {
                CardName = card.Name,
                Brand = card.Brand,
                Month = month,
                Year = year,
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalTransactions = expenses.Count,
                CardLimit = card.CardLimit,
                Expenses = expenses.Select(MapToExpenseResponse).ToList()
            };
        }

        public async Task<List<CategoryAnalysisResponse>> GetCategoryAnalysisAsync(int creditCardId, int month, int year)
        {
            var expenses = await _repository.GetExpensesByCardAsync(creditCardId, month, year);
            var totalAmount = expenses.Sum(e => e.Amount);

            return expenses
                .GroupBy(e => e.Category ?? "Sem Categoria")
                .Select(g => new CategoryAnalysisResponse
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(e => e.Amount),
                    TransactionCount = g.Count(),
                    AverageAmount = g.Average(e => e.Amount),
                    Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
                })
                .OrderByDescending(c => c.TotalAmount)
                .ToList();
        }

        private CreditCardResponse MapToResponse(CreditCard creditCard)
        {
            return new CreditCardResponse
            {
                Id = creditCard.Id,
                Name = creditCard.Name,
                Brand = creditCard.Brand,
                CardLimit = creditCard.CardLimit,
                ClosingDay = creditCard.ClosingDay,
                DueDay = creditCard.DueDay,
                IsActive = creditCard.IsActive,
                CreatedAt = creditCard.CreatedAt,
                UpdatedAt = creditCard.UpdatedAt
            };
        }

        private CreditCardExpenseResponse MapToExpenseResponse(CreditCardExpense expense)
        {
            return new CreditCardExpenseResponse
            {
                Id = expense.Id,
                CreditCardId = expense.CreditCardId,
                PurchaseDate = expense.PurchaseDate,
                Description = expense.Description,
                Amount = expense.Amount,
                Category = expense.Category,
                Month = expense.Month,
                Year = expense.Year,
                Status = expense.Status,
                CreatedAt = expense.CreatedAt
            };
        }

        private string CategorizeExpense(string description)
        {
            var desc = description.ToLower();

            if (desc.Contains("mercado") || desc.Contains("supermercado") || desc.Contains("cometa") ||
                desc.Contains("mcdonald") || desc.Contains("buffet") || desc.Contains("restaurante") ||
                desc.Contains("lanchonete") || desc.Contains("padaria") || desc.Contains("acai") ||
                desc.Contains("espeto") || desc.Contains("pitanga"))
                return "Alimentação";

            if (desc.Contains("uber") || desc.Contains("posto") || desc.Contains("gasolina") ||
                desc.Contains("combustivel") || desc.Contains("taxi") || desc.Contains("99") ||
                desc.Contains("estacionament"))
                return "Transporte";

            if (desc.Contains("farmacia") || desc.Contains("pague menos") || desc.Contains("clinica") ||
                desc.Contains("droga") || desc.Contains("medic") || desc.Contains("hospital") ||
                desc.Contains("consulta") || desc.Contains("academia") || desc.Contains("musculacao"))
                return "Saúde";

            if (desc.Contains("pet") || desc.Contains("veterinari") || desc.Contains("animal"))
                return "Pets";

            if (desc.Contains("barbearia") || desc.Contains("salao") || desc.Contains("cabelo") ||
                desc.Contains("manicure") || desc.Contains("estetica"))
                return "Beleza";

            if (desc.Contains("amazon") || desc.Contains("mercadolivre") || desc.Contains("mercadolibre") ||
                desc.Contains("shopee") || desc.Contains("aliexpress") || desc.Contains("apple"))
                return "Compras Online";

            if (desc.Contains("claro") || desc.Contains("vivo") || desc.Contains("tim") ||
                desc.Contains("oi") || desc.Contains("telefone") || desc.Contains("internet"))
                return "Telefonia";

            if (desc.Contains("calcado") || desc.Contains("roupa") || desc.Contains("loja") ||
                desc.Contains("moda") || desc.Contains("tenis"))
                return "Vestuário";

            if (desc.Contains("cinema") || desc.Contains("teatro") || desc.Contains("show") ||
                desc.Contains("ingresso") || desc.Contains("viagem") || desc.Contains("hotel") ||
                desc.Contains("pousada"))
                return "Lazer";

            if (desc.Contains("seguro") || desc.Contains("azul seguros"))
                return "Seguros";

            if (desc.Contains("curso") || desc.Contains("escola") || desc.Contains("faculdade") ||
                desc.Contains("livro") || desc.Contains("material escolar"))
                return "Educação";

            return "Outros";
        }
    }

    // Classe para mapear CSV (precisa ser pública para CsvHelper)
    public class CsvExpenseRecord
    {
        [Name("date")]
        public string Date { get; set; }

        [Name("title")]
        public string Title { get; set; }

        [Name("amount")]
        public string Amount { get; set; }
    }
}
