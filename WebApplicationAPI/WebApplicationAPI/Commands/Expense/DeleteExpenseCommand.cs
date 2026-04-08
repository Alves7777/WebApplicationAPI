using MediatR;

namespace WebApplicationAPI.Commands.Expense
{
    public class DeleteExpenseCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DeleteExpenseCommand(int id)
        {
            Id = id;
        }
    }
}