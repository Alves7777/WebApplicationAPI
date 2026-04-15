using MediatR;
using WebApplicationAPI.DTO;

namespace WebApplicationAPI.Queries.Summary
{
    public class GetFinancialSummaryQuery : IRequest<SummaryResponse>
    {
        public int UserId { get; set; } // ? Adicionado UserId
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Salary { get; set; }
        public decimal Reserve { get; set; }
    }
}