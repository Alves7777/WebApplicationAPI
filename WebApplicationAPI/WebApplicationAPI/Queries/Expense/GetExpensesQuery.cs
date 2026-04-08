using MediatR;
using WebApplicationAPI.DTO;
using System.Collections.Generic;

namespace WebApplicationAPI.Queries.Expense
{
    public class GetExpensesQuery : IRequest<List<ExpenseResponse>>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public string? PaymentMethod { get; set; }
    }
}