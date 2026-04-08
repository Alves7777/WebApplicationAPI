using MediatR;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Commands.Expense
{
    public class PatchExpenseCommand : IRequest<ExpenseResponse>
    {
        public int Id { get; set; }
        public PatchExpenseRequest Request { get; set; }

        public PatchExpenseCommand(int id, PatchExpenseRequest request)
        {
            Id = id;
            Request = request;
        }
    }
}
