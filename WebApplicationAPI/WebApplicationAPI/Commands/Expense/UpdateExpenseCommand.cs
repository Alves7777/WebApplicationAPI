using MediatR;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Commands.Expense
{
    public class UpdateExpenseCommand : IRequest<ExpenseResponse>
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public UpdateExpenseRequest Request { get; set; }
        public UpdateExpenseCommand(int id, int userId, UpdateExpenseRequest request)
        {
            Id = id;
            Request = request;
            UserId = userId;
        }
    }
}