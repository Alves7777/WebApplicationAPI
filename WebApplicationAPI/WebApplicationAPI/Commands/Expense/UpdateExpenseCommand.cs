using MediatR;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Commands.Expense
{
    public class UpdateExpenseCommand : IRequest<ExpenseResponse>
    {
        public int Id { get; set; }
        public UpdateExpenseRequest Request { get; set; }
        public UpdateExpenseCommand(int id, UpdateExpenseRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}