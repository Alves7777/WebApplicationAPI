using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Queries.Expense;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Helpers;

namespace WebApplicationAPI.Handlers.Expense
{
    public class GetExpensesHandler : IRequestHandler<GetExpensesQuery, List<ExpenseResponse>>
    {
        private readonly IExpenseRepository _repository;
        private readonly UserContext _userContext;

        public GetExpensesHandler(IExpenseRepository repository, UserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<List<ExpenseResponse>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId(); // ? Pega do token JWT

            // ? Busca apenas expenses do usuário logado
            var expenses = await _repository.GetExpensesByUserIdAsync(userId, request.Month, request.Year, request.Category, request.Status, request.PaymentMethod);

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