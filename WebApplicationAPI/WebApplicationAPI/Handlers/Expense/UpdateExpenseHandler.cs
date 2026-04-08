using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Handlers.Expense
{
    public class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, ExpenseResponse>
    {
        private readonly IExpenseRepository _repository;

        public UpdateExpenseHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }

        public async Task<ExpenseResponse> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetExpenseByIdAsync(request.Id);
            if (existing == null) return null;

            var req = request.Request;
            var updated = new Models.Expense
            {
                Id = request.Id,
                Month = req.Month,
                Year = req.Year,
                Description = req.Description,
                Amount = req.Amount,
                Category = req.Category,
                Status = req.Status,
                PaymentMethod = req.PaymentMethod,
                CreatedAt = existing.CreatedAt
            };

            var result = await _repository.UpdateExpenseAsync(updated);

            return new ExpenseResponse
            {
                Id = result.Id,
                Month = result.Month,
                Year = result.Year,
                Description = result.Description,
                Amount = result.Amount,
                Category = result.Category,
                Status = result.Status,
                PaymentMethod = result.PaymentMethod,
                CreatedAt = result.CreatedAt
            };
        }
    }
}