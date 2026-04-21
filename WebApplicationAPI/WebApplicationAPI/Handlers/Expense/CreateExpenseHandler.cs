using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Helpers;

namespace WebApplicationAPI.Handlers.Expense
{
    public class CreateExpenseHandler : IRequestHandler<CreateExpenseCommand, ExpenseResponse>
    {
        private readonly IExpenseRepository _repository;
        private readonly UserContext _userContext;

        public CreateExpenseHandler(IExpenseRepository repository, UserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<ExpenseResponse> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId(); // ? Pega do token JWT

            var req = request.Request;
            var expense = new Models.Expense
            {
                UserId = userId, // ? Vincula ao usuário logado
                Month = req.Month,
                Year = req.Year,
                Description = req.Description,
                Amount = req.Amount,
                Category = req.Category,
                Status = req.Status,
                PaymentMethod = req.PaymentMethod,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.CreateExpenseAsync(expense);

            return new ExpenseResponse
            {
                Id = created.Id,
                Month = created.Month,
                Year = created.Year,
                Description = created.Description,
                Amount = created.Amount,
                Category = created.Category,
                Status = created.Status,
                PaymentMethod = created.PaymentMethod,
                CreatedAt = created.CreatedAt
            };
        }
    }
}