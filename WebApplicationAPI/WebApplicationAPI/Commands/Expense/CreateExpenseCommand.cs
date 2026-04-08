using MediatR;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Commands.Expense
{
    public class CreateExpenseCommand : IRequest<ExpenseResponse>
    {
        public CreateExpenseRequest Request { get; set; }
        public CreateExpenseCommand(CreateExpenseRequest request)
        {
            Request = request;
        }
    }
}