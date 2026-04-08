using System.Threading;
using System.Threading.Tasks;
using MediatR;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.Repositories.Interfaces;

namespace WebApplicationAPI.Handlers.Expense
{
    public class DeleteExpenseHandler : IRequestHandler<DeleteExpenseCommand, bool>
    {
        private readonly IExpenseRepository _repository;
        public DeleteExpenseHandler(IExpenseRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteExpenseAsync(request.Id);
        }
    }
}