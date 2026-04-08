using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Queries.Expense;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Handlers.Expense
{
    public class GetExpensesHandler : IRequestHandler<GetExpensesQuery, List<ExpenseResponse>>
    {
        private readonly IExpenseRepository _repository;

        public GetExpensesHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ExpenseResponse>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var expenses = await _repository.GetExpensesAsync(request.Month, request.Year, request.Category, request.Status, request.PaymentMethod);
            
            return expenses.Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Month = e.Month,
                Year = e.Year,
                Description = e.Description,
                Amount = e.Amount,
                Category = e.Category,
                Status = e.Status,
                PaymentMethod = e.PaymentMethod,
                CreatedAt = e.CreatedAt
            }).ToList();
        }
    }
}