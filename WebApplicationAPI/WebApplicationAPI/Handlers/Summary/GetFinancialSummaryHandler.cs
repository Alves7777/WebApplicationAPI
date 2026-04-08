using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Queries.Summary;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Handlers.Summary
{
    public class GetFinancialSummaryHandler : IRequestHandler<GetFinancialSummaryQuery, SummaryResponse>
    {
        private readonly IExpenseRepository _repository;
        private readonly IFinancialCalculatorService _calculator;
        public GetFinancialSummaryHandler(IExpenseRepository repository, IFinancialCalculatorService calculator)
        {
            _repository = repository;
            _calculator = calculator;
        }
        public async Task<SummaryResponse> Handle(GetFinancialSummaryQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetExpensesAsync(request.Month, request.Year, null, null, null);
            var totalExpenses = _calculator.GetTotalExpenses(expenses);
            var totalByCategory = _calculator.GetTotalByCategory(expenses);
            var totalByStatus = _calculator.GetTotalByStatus(expenses);
            var balance = _calculator.CalculateBalance(request.Salary, totalExpenses);
            var availableToSpend = _calculator.CalculateAvailableToSpend(balance, request.Reserve);
            return new SummaryResponse
            {
                TotalExpenses = totalExpenses,
                Balance = balance,
                AvailableToSpend = availableToSpend,
                TotalByCategory = totalByCategory,
                TotalByStatus = totalByStatus
            };
        }
    }
}