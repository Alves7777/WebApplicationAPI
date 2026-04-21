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
    public class UpdateExpenseHandler : IRequestHandler<UpdateExpenseCommand, ExpenseResponse>
    {
        private readonly IExpenseRepository _repository;
        private readonly UserContext _userContext;

        public UpdateExpenseHandler(IExpenseRepository repository, UserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<ExpenseResponse> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var existing = await _repository.GetExpenseByIdAsync(request.Id, userId);
            if (existing == null)
            {
                return null;
            }

            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta despesa");
            }

            var req = request.Request;
            var updated = new Models.Expense
            {
                Id = request.Id,
                UserId = existing.UserId,
                Month = req.Month,
                Year = req.Year,
                Description = req.Description,
                Amount = req.Amount,
                Category = req.Category,
                Status = req.Status,
                PaymentMethod = req.PaymentMethod,
                UpdatedBy = existing.UserId
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