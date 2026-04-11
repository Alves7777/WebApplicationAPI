using System;
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
        private readonly IMonthlyFinancialRepository _monthlyFinancialRepository;
        private readonly IExpenseRepository _expenseRepository;

        public CreditCardService(
            ICreditCardRepository repository, 
            IMonthlyFinancialRepository monthlyFinancialRepository,
            IExpenseRepository expenseRepository)
        {
            _repository = repository;
            _monthlyFinancialRepository = monthlyFinancialRepository;
            _expenseRepository = expenseRepository;
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

                        var exists = await _repository.ExpenseExistsAsync(
                            creditCardId,
                            purchaseDate,
                            record.Title,
                            amount
                        );

                        if (exists)
                        {
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

            var closingDay = card.ClosingDay ?? 28;

            var closingMonth = month;
            var closingYear = year;

            if (month == 1)
            {
                closingMonth = 12;
                closingYear = year - 1;
            }
            else
            {
                closingMonth = month - 1;
            }

            var (periodStart, periodEnd) = CalculateBillingPeriod(closingYear, closingMonth, closingDay);

            var expenses = await _repository.GetExpensesByPeriodAsync(creditCardId, periodStart, periodEnd);

            return new CreditCardStatementResponse
            {
                CardName = card.Name,
                Brand = card.Brand,
                Month = month,
                Year = year,
                BillingPeriodStart = periodStart,
                BillingPeriodEnd = periodEnd,
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalTransactions = expenses.Count,
                CardLimit = card.CardLimit,
                Expenses = expenses.Select(MapToExpenseResponse).ToList()
            };
        }

        public async Task<List<CategoryAnalysisResponse>> GetCategoryAnalysisAsync(int creditCardId, int month, int year)
        {
            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var closingDay = card.ClosingDay ?? 28;

            var closingMonth = month;
            var closingYear = year;

            if (month == 1)
            {
                closingMonth = 12;
                closingYear = year - 1;
            }
            else
            {
                closingMonth = month - 1;
            }

            var (periodStart, periodEnd) = CalculateBillingPeriod(closingYear, closingMonth, closingDay);

            var expenses = await _repository.GetExpensesByPeriodAsync(creditCardId, periodStart, periodEnd);
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

        public async Task<CreditCardStatementResponse> GetStatementByPeriodAsync(int creditCardId, DateTime startDate, DateTime endDate)
        {
            var daysDifference = (endDate - startDate).TotalDays;
            if (daysDifference > 30)
            {
                throw new InvalidOperationException("O período não pode exceder 30 dias");
            }

            if (daysDifference < 0)
            {
                throw new InvalidOperationException("A data final não pode ser anterior à data inicial");
            }

            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var expenses = await _repository.GetExpensesByPeriodAsync(creditCardId, startDate, endDate);

            return new CreditCardStatementResponse
            {
                CardName = card.Name,
                Brand = card.Brand,
                Month = startDate.Month,
                Year = startDate.Year,
                TotalAmount = expenses.Sum(e => e.Amount),
                TotalTransactions = expenses.Count,
                CardLimit = card.CardLimit,
                Expenses = expenses.Select(MapToExpenseResponse).ToList()
            };
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

        public async Task<SimulatePurchaseResponse> SimulatePurchaseAsync(int creditCardId, SimulatePurchaseRequest request)
        {
            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var installmentAmount = request.Amount / request.Installments;
            var currentDate = DateTime.Now;
            var analysis = new List<MonthAnalysis>();
            int riskCount = 0;
            int safeCount = 0;
            var riskMonths = new List<string>();
            decimal totalSalary = 0;
            decimal totalExpenses = 0;
            decimal totalAvailable = 0;

            var closingDay = card.ClosingDay ?? 28;

            for (int i = 0; i < request.Installments; i++)
            {
                var targetDate = currentDate.AddMonths(i);
                var targetMonth = targetDate.Month;
                var targetYear = targetDate.Year;
                var monthName = new DateTime(targetYear, targetMonth, 1).ToString("MMMM", new System.Globalization.CultureInfo("pt-BR"));

                var (periodStart, periodEnd) = CalculateBillingPeriod(targetYear, targetMonth, closingDay);

                var monthlyControl = await GetMonthlyControlData(targetMonth, targetYear);

                var creditCardExpenses = await _repository.GetExpensesByPeriodAsync(creditCardId, periodStart, periodEnd);
                var creditCardTotal = creditCardExpenses.Sum(e => e.Amount);

                var otherExpensesTotal = await _expenseRepository.GetTotalExpensesByMonthExcludingCategoryAsync(
                    targetMonth, 
                    targetYear, 
                    "Cartão de Crédito"
                );

                var existingInstallments = await _repository.GetActiveInstallmentsByMonthAsync(creditCardId, targetMonth, targetYear);
                var existingInstallmentsTotal = existingInstallments.Sum(inst => inst.InstallmentAmount);

                var salary = monthlyControl?.SalaryTotal ?? 0;
                var reserve = monthlyControl?.Reserve ?? 0;
                var hasControl = monthlyControl != null;

                var totalCommitment = creditCardTotal + otherExpensesTotal + existingInstallmentsTotal + installmentAmount;
                var availableAfter = salary - totalCommitment - reserve;

                string status;
                string alert = null;

                if (!hasControl)
                {
                    status = "SEM_CONTROLE";
                    alert = "Mês sem controle financeiro cadastrado";
                    riskCount++;
                    riskMonths.Add($"{monthName}/{targetYear}");
                }
                else if (availableAfter < 0)
                {
                    status = "RISCO";
                    alert = $"Saldo negativo de {availableAfter:C}";
                    riskCount++;
                    riskMonths.Add($"{monthName}/{targetYear}");
                }
                else if (availableAfter < 200)
                {
                    status = "ATENÇÃO";
                    alert = $"Sobra apenas {availableAfter:C}";
                    safeCount++;
                }
                else
                {
                    status = "OK";
                    safeCount++;
                }

                totalSalary += salary;
                totalExpenses += (creditCardTotal + otherExpensesTotal);
                totalAvailable += availableAfter;

                analysis.Add(new MonthAnalysis
                {
                    Month = targetMonth,
                    Year = targetYear,
                    MonthName = monthName,
                    BillingPeriodStart = periodStart,
                    BillingPeriodEnd = periodEnd,
                    Salary = salary,
                    CreditCardExpenses = creditCardTotal,
                    OtherExpenses = otherExpensesTotal,
                    CurrentExpenses = creditCardTotal + otherExpensesTotal,
                    ProjectedExpenses = creditCardTotal + otherExpensesTotal,
                    Reserve = reserve,
                    ExistingInstallments = existingInstallmentsTotal,
                    NewInstallment = installmentAmount,
                    TotalCommitment = totalCommitment,
                    AvailableAfter = availableAfter,
                    Status = status,
                    Alert = alert,
                    HasMonthlyControl = hasControl
                });
            }

            string recommendation;
            bool canAfford;

            if (riskCount == 0)
            {
                recommendation = "PODE COMPRAR";
                canAfford = true;
            }
            else if (riskCount <= 2)
            {
                recommendation = "ATENÇÃO - Aperte o cinto nos meses destacados";
                canAfford = true;
            }
            else
            {
                recommendation = "NÃO RECOMENDADO - Você não terá condições de pagar";
                canAfford = false;
            }

            return new SimulatePurchaseResponse
            {
                CanAfford = canAfford,
                Recommendation = recommendation,
                PurchaseAmount = request.Amount,
                InstallmentAmount = installmentAmount,
                Installments = request.Installments,
                Analysis = analysis,
                Summary = new SimulationSummary
                {
                    TotalImpact = request.Amount,
                    MonthlyImpact = installmentAmount,
                    RiskMonthsCount = riskCount,
                    SafeMonthsCount = safeCount,
                    RiskMonths = riskMonths,
                    AverageSalary = request.Installments > 0 ? totalSalary / request.Installments : 0,
                    AverageExpenses = request.Installments > 0 ? totalExpenses / request.Installments : 0,
                    AverageAvailable = request.Installments > 0 ? totalAvailable / request.Installments : 0
                }
            };
        }

        public async Task<InstallmentPurchaseResponse> ConfirmPurchaseAsync(int creditCardId, ConfirmPurchaseRequest request)
        {
            var card = await _repository.GetByIdAsync(creditCardId);
            if (card == null)
            {
                throw new InvalidOperationException("Cartão não encontrado");
            }

            var currentDate = DateTime.Now;
            var installmentAmount = request.Amount / request.Installments;

            var purchase = new InstallmentPurchase
            {
                CreditCardId = creditCardId,
                Description = request.Description,
                TotalAmount = request.Amount,
                InstallmentCount = request.Installments,
                InstallmentAmount = installmentAmount,
                FirstInstallmentMonth = currentDate.Month,
                FirstInstallmentYear = currentDate.Year,
                Status = "ATIVA",
                CreatedAt = DateTime.UtcNow
            };

            var id = await _repository.CreateInstallmentPurchaseAsync(purchase);
            purchase.Id = id;

            return new InstallmentPurchaseResponse
            {
                Id = purchase.Id,
                CreditCardId = purchase.CreditCardId,
                Description = purchase.Description,
                TotalAmount = purchase.TotalAmount,
                InstallmentCount = purchase.InstallmentCount,
                InstallmentAmount = purchase.InstallmentAmount,
                FirstInstallmentMonth = purchase.FirstInstallmentMonth,
                FirstInstallmentYear = purchase.FirstInstallmentYear,
                Status = purchase.Status,
                CreatedAt = purchase.CreatedAt,
                RemainingInstallments = purchase.InstallmentCount,
                RemainingAmount = purchase.TotalAmount
            };
        }

        public async Task<List<InstallmentPurchaseResponse>> GetInstallmentPurchasesAsync(int creditCardId)
        {
            var purchases = await _repository.GetAllInstallmentPurchasesAsync(creditCardId);
            var currentDate = DateTime.Now;

            return purchases.Select(p =>
            {
                var monthsPassed = ((currentDate.Year - p.FirstInstallmentYear) * 12) + (currentDate.Month - p.FirstInstallmentMonth);
                var remaining = Math.Max(0, p.InstallmentCount - monthsPassed);

                return new InstallmentPurchaseResponse
                {
                    Id = p.Id,
                    CreditCardId = p.CreditCardId,
                    Description = p.Description,
                    TotalAmount = p.TotalAmount,
                    InstallmentCount = p.InstallmentCount,
                    InstallmentAmount = p.InstallmentAmount,
                    FirstInstallmentMonth = p.FirstInstallmentMonth,
                    FirstInstallmentYear = p.FirstInstallmentYear,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    RemainingInstallments = remaining,
                    RemainingAmount = remaining * p.InstallmentAmount
                };
            }).ToList();
        }

        private async Task<MonthlyFinancialControlData> GetMonthlyControlData(int month, int year)
        {
            var control = await _monthlyFinancialRepository.GetByYearAndMonthAsync(year, month);

            if (control == null)
                return null;

            return new MonthlyFinancialControlData
            {
                SalaryTotal = control.Money + control.RV + control.Debit + control.Others,
                Reserve = control.Reserve
            };
        }

        private (DateTime PeriodStart, DateTime PeriodEnd) CalculateBillingPeriod(int year, int month, int closingDay)
        {
            DateTime periodEnd;
            DateTime periodStart;

            if (month == 1)
            {
                periodStart = new DateTime(year - 1, 12, Math.Min(closingDay, DateTime.DaysInMonth(year - 1, 12)));
                periodEnd = new DateTime(year, month, Math.Min(closingDay, DateTime.DaysInMonth(year, month))).AddDays(-1);
            }
            else
            {
                periodStart = new DateTime(year, month - 1, Math.Min(closingDay, DateTime.DaysInMonth(year, month - 1)));
                periodEnd = new DateTime(year, month, Math.Min(closingDay, DateTime.DaysInMonth(year, month))).AddDays(-1);
            }

            return (periodStart, periodEnd);
        }
    }

    public class MonthlyFinancialControlData
    {
        public decimal SalaryTotal { get; set; }
        public decimal Reserve { get; set; }
    }

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
