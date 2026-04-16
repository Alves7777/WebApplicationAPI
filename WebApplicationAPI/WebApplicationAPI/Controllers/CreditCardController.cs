using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Services.Interfaces;
using WebApplicationAPI.Extensions;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/creditcard")]
    [Authorize]
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
                var userId = this.GetUserId();
                var result = await _service.CreateAsync(userId, request);
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
                var userId = this.GetUserId();
                var result = await _service.UpdateAsync(id, userId, request);
                return Ok(ApiResponse<CreditCardResponse>.Success(result, "Cartão atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message));
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
                var userId = this.GetUserId();
                var result = await _service.DeleteAsync(id, userId);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.Fail("Cartão não encontrado"));
                }
                return Ok(ApiResponse.Success("Cartão deletado com sucesso"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message));
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
                var userId = this.GetUserId();
                var result = await _service.GetAllAsync(userId);
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
                var userId = this.GetUserId();
                var result = await _service.GetByIdAsync(id, userId);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Cartão não encontrado"));
                }
                return Ok(ApiResponse<CreditCardResponse>.Success(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message));
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
                var userId = this.GetUserId();
                var result = await _service.CreateExpenseAsync(id, userId, request);
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
                var userId = this.GetUserId();
                var result = await _service.UpdateExpenseAsync(expenseId, userId, request);
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
                var userId = this.GetUserId();
                var result = await _service.DeleteExpenseAsync(expenseId, userId);
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
                var userId = this.GetUserId();
                var result = await _service.GetExpensesByCardAsync(id, userId, month, year, category);
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
                var userId = this.GetUserId();
                var result = await _service.GetExpenseByIdAsync(expenseId, userId);
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

                var userId = this.GetUserId();
                using var stream = file.OpenReadStream();
                var result = await _service.ImportCsvAsync(id, userId, stream);

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
                var userId = this.GetUserId();
                var result = await _service.GetStatementAsync(id, userId, month, year);
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
                var userId = this.GetUserId();
                var result = await _service.GetCategoryAnalysisAsync(id, userId, month, year);
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
                var userId = this.GetUserId();
                var result = await _service.GetStatementByPeriodAsync(id, userId, startDate, endDate);
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

        [HttpPost("{id}/simulate-purchase")]
        [ProducesResponseType(typeof(ApiResponse<SimulatePurchaseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SimulatePurchase(int id, [FromBody] SimulatePurchaseRequest request)
        {
            try
            {
                var userId = this.GetUserId();
                var result = await _service.SimulatePurchaseAsync(id, userId, request);
                return Ok(ApiResponse<SimulatePurchaseResponse>.Success(result, $"Simulação de compra: {result.Recommendation}"));
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

        [HttpPost("{id}/confirm-purchase")]
        [ProducesResponseType(typeof(ApiResponse<InstallmentPurchaseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmPurchase(int id, [FromBody] ConfirmPurchaseRequest request)
        {
            try
            {
                var userId = this.GetUserId();
                var result = await _service.ConfirmPurchaseAsync(id, userId, request);
                return StatusCode(201, ApiResponse<InstallmentPurchaseResponse>.Success(result, "Compra parcelada registrada com sucesso"));
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

        [HttpGet("{id}/installment-purchases")]
        [ProducesResponseType(typeof(ApiResponse<List<InstallmentPurchaseResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInstallmentPurchases(int id)
        {
            try
            {
                var userId = this.GetUserId();
                var result = await _service.GetInstallmentPurchasesAsync(id, userId);
                return Ok(ApiResponse<List<InstallmentPurchaseResponse>>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }
    }
}
