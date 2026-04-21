using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Helpers;

namespace WebApplicationAPI.Handlers.Expense
{
    public class PatchExpenseHandler : IRequestHandler<PatchExpenseCommand, ExpenseResponse>
    {
        private readonly IExpenseRepository _repository;
        private readonly UserContext _userContext;

        public PatchExpenseHandler(IExpenseRepository repository, UserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<ExpenseResponse> Handle(PatchExpenseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId();

            var existing = await _repository.GetExpenseByIdAsync(request.Id, userId);
            if (existing == null) return null;

            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para atualizar esta despesa");
            }

            var patch = request.Request;

            if (patch.Month.HasValue) existing.Month = patch.Month.Value;
            if (patch.Year.HasValue) existing.Year = patch.Year.Value;
            if (!string.IsNullOrEmpty(patch.Description)) existing.Description = patch.Description;
            if (patch.Amount.HasValue) existing.Amount = patch.Amount.Value;
            if (!string.IsNullOrEmpty(patch.Category)) existing.Category = patch.Category;
            if (!string.IsNullOrEmpty(patch.Status)) existing.Status = patch.Status;
            if (!string.IsNullOrEmpty(patch.PaymentMethod)) existing.PaymentMethod = patch.PaymentMethod;

            var result = await _repository.UpdateExpenseAsync(existing);

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
