using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/creditcard")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _service;

        public CreditCardController(ICreditCardService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreditCardResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCreditCardRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return StatusCode(201, ApiResponse<CreditCardResponse>.Success(result, "Cartão criado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCreditCardRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(ApiResponse<CreditCardResponse>.Success(result, "Cartão atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.Fail("Cartão não encontrado"));
                }
                return Ok(ApiResponse.Success("Cartão deletado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CreditCardResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(ApiResponse<List<CreditCardResponse>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Cartão não encontrado"));
                }
                return Ok(ApiResponse<CreditCardResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpPost("{id}/expenses")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardExpenseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateExpense(int id, [FromBody] CreateCreditCardExpenseRequest request)
        {
            try
            {
                var result = await _service.CreateExpenseAsync(id, request);
                return StatusCode(201, ApiResponse<CreditCardExpenseResponse>.Success(result, "Despesa criada com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpPut("expenses/{expenseId}")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardExpenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateExpense(int expenseId, [FromBody] UpdateCreditCardExpenseRequest request)
        {
            try
            {
                var result = await _service.UpdateExpenseAsync(expenseId, request);
                return Ok(ApiResponse<CreditCardExpenseResponse>.Success(result, "Despesa atualizada com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpDelete("expenses/{expenseId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            try
            {
                var result = await _service.DeleteExpenseAsync(expenseId);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.Fail("Despesa não encontrada"));
                }
                return Ok(ApiResponse.Success("Despesa deletada com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}/expenses")]
        [ProducesResponseType(typeof(ApiResponse<List<CreditCardExpenseResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExpensesByCard(int id, [FromQuery] int? month, [FromQuery] int? year, [FromQuery] string? category)
        {
            try
            {
                var result = await _service.GetExpensesByCardAsync(id, month, year, category);
                return Ok(ApiResponse<List<CreditCardExpenseResponse>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("expenses/{expenseId}")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardExpenseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExpenseById(int expenseId)
        {
            try
            {
                var result = await _service.GetExpenseByIdAsync(expenseId);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Despesa não encontrada"));
                }
                return Ok(ApiResponse<CreditCardExpenseResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpPost("{id}/import-csv")]
        [ProducesResponseType(typeof(ApiResponse<CsvImportResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ImportCsv(int id, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(ApiResponse<object>.Fail("Arquivo não fornecido ou vazio"));
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(ApiResponse<object>.Fail("Apenas arquivos CSV são permitidos"));
                }

                using var stream = file.OpenReadStream();
                var result = await _service.ImportCsvAsync(id, stream);

                return Ok(ApiResponse<CsvImportResult>.Success(result, $"{result.ImportedRecords} despesas importadas com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro ao importar CSV: {ex.Message}"));
            }
        }

        [HttpGet("{id}/statement")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardStatementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatement(int id, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _service.GetStatementAsync(id, month, year);
                return Ok(ApiResponse<CreditCardStatementResponse>.Success(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}/by-category")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryAnalysisResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByCategory(int id, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var result = await _service.GetCategoryAnalysisAsync(id, month, year);
                return Ok(ApiResponse<List<CategoryAnalysisResponse>>.Success(result));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}/statement-period")]
        [ProducesResponseType(typeof(ApiResponse<CreditCardStatementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStatementByPeriod(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _service.GetStatementByPeriodAsync(id, startDate, endDate);
                return Ok(ApiResponse<CreditCardStatementResponse>.Success(result, $"Extrato de {startDate:dd/MM/yyyy} até {endDate:dd/MM/yyyy}"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }
    }
}
