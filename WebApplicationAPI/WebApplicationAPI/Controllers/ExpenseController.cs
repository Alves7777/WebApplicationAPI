using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApplicationAPI.Commands.Expense;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Extensions;
using WebApplicationAPI.Queries.Expense;
using WebApplicationAPI.Queries.Summary;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
            // O UserId é resolvido no handler via UserContext
            var result = await _mediator.Send(new CreateExpenseCommand(request));
            return StatusCode(201, ApiResponse<ExpenseResponse>.Success(result, "Despesa criada com sucesso"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            var userId = this.GetUserId();

            var result = await _mediator.Send(new UpdateExpenseCommand(id, userId, request));
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
            // O UserId é resolvido no handler via UserContext
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

        [HttpPost("import-csv")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.Fail("Nenhum arquivo foi enviado."));
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResponse<object>.Fail("O arquivo deve ser um CSV."));
            }

            var importResult = new ImportCsvResult
            {
                TotalRecords = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                Errors = new List<string>()
            };

            try
            {
                using var reader = new StreamReader(file.OpenReadStream());
                var line = await reader.ReadLineAsync(); // Skip header
                var lineNumber = 1;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    lineNumber++;
                    importResult.TotalRecords++;

                    try
                    {
                        var parts = line.Split(new[] { ',', ';' }, StringSplitOptions.None);

                        if (parts.Length < 8)
                        {
                            importResult.Errors.Add($"Linha {lineNumber}: Formato inválido (menos de 8 colunas)");
                            importResult.ErrorCount++;
                            continue;
                        }

                        var request = new CreateExpenseRequest
                        {
                            Category = parts[0].Trim(),
                            Amount = decimal.Parse(parts[1].Trim().Replace(".", ",")),
                            Description = parts[2].Trim(),
                            Month = int.Parse(parts[4].Trim()),
                            Year = int.Parse(parts[5].Trim()),
                            Status = parts[6].Trim(),
                            PaymentMethod = parts[7].Trim()
                        };

                        await _mediator.Send(new CreateExpenseCommand(request));
                        importResult.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        importResult.Errors.Add($"Linha {lineNumber}: {ex.Message}");
                        importResult.ErrorCount++;
                    }
                }

                return Ok(ApiResponse<ImportCsvResult>.Success(importResult, 
                    $"Importação concluída. {importResult.SuccessCount} registros importados com sucesso."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Fail($"Erro ao processar arquivo: {ex.Message}"));
            }
        }
    }

    public class ImportCsvResult
    {
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> Errors { get; set; }
    }
}