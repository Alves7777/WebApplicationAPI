using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Queries.Expense;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ExpenseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExpenseRequest request)
        {
            var result = await _mediator.Send(new CreateExpenseCommand(request));
            return StatusCode(201, ApiResponse<ExpenseResponse>.Success(result, "Despesa criada com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            var result = await _mediator.Send(new UpdateExpenseCommand(id, request));
            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail("Despesa não encontrada"));
            }

            return Ok(ApiResponse<ExpenseResponse>.Success(result, "Despesa atualizada com sucesso"));
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, [FromBody] PatchExpenseRequest request)
        {
            var result = await _mediator.Send(new PatchExpenseCommand(id, request));
            if (result == null)
            {
                return NotFound(ApiResponse<object>.Fail("Despesa não encontrada"));
            }

            return Ok(ApiResponse<ExpenseResponse>.Success(result, "Despesa atualizada parcialmente"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteExpenseCommand(id));
            if (!result)
            {
                return NotFound(ApiResponse<object>.Fail("Despesa não encontrada"));
            }
            return Ok(ApiResponse.Success("Despesa deletada com sucesso"));
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetExpensesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(ApiResponse<List<ExpenseResponse>>.Success(result));
        }

        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] int? month, [FromQuery] int? year, [FromQuery] string category)
        {
            var query = new GetExpensesQuery
            {
                Month = month,
                Year = year,
                Category = category,
                Status = null,
                PaymentMethod = null
            };
            var expenses = await _mediator.Send(query);

            var report = new
            {
                Filters = new
                {
                    Month = month,
                    Year = year,
                    Category = category
                },
                Summary = new
                {
                    TotalExpenses = expenses.Count,
                    TotalAmount = expenses.Sum(e => e.Amount),
                    AverageAmount = expenses.Any() ? expenses.Average(e => e.Amount) : 0,
                    MinAmount = expenses.Any() ? expenses.Min(e => e.Amount) : 0,
                    MaxAmount = expenses.Any() ? expenses.Max(e => e.Amount) : 0
                },
                ByCategory = expenses
                    .GroupBy(e => e.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(e => e.Amount),
                        Percentage = expenses.Sum(e => e.Amount) > 0 
                            ? Math.Round((g.Sum(e => e.Amount) / expenses.Sum(e => e.Amount)) * 100, 2)
                            : 0
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList(),
                ByStatus = expenses
                    .GroupBy(e => e.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(e => e.Amount)
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList(),
                ByPaymentMethod = expenses
                    .GroupBy(e => e.PaymentMethod)
                    .Select(g => new
                    {
                        PaymentMethod = g.Key,
                        Count = g.Count(),
                        Total = g.Sum(e => e.Amount)
                    })
                    .OrderByDescending(x => x.Total)
                    .ToList(),
                Expenses = expenses
            };

            return Ok(report);
        }
    }
}