using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Helpers;

namespace WebApplicationAPI.Handlers.Expense
{
    public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, bool>
    {
        private readonly IExpenseRepository _repository;
        private readonly UserContext _userContext;

        public DeleteExpenseHandler(IExpenseRepository repository, UserContext userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContext.GetCurrentUserId(); // ? Pega do token JWT

            var existing = await _repository.GetExpenseByIdAsync(request.Id, userId);
            if (existing == null)
            {
                return false;
            }

            // ? Validar ownership - apenas dono pode deletar
            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para deletar esta despesa");
            }

            return await _repository.DeleteExpenseAsync(request.Id, userId);
        }
    }
}