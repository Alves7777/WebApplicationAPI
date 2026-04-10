using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/v1/monthly-financial")]
    public class MonthlyFinancialController : ControllerBase
    {
        private readonly IMonthlyFinancialService _service;

        public MonthlyFinancialController(IMonthlyFinancialService service)
        {
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MonthlyFinancialResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMonthlyFinancialRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return StatusCode(201, ApiResponse<MonthlyFinancialResponse>.Success(result, "Controle mensal criado com sucesso"));
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
        [ProducesResponseType(typeof(ApiResponse<MonthlyFinancialResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonthlyFinancialRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(ApiResponse<MonthlyFinancialResponse>.Success(result, "Controle mensal atualizado"));
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
                    return NotFound(ApiResponse<object>.Fail("Registro não encontrado"));
                }
                return Ok(ApiResponse.Success("Registro deletado com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<MonthlyFinancialResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? year, [FromQuery] int? month)
        {
            try
            {
                if (year.HasValue && month.HasValue)
                {
                    var specific = await _service.GetByYearAndMonthAsync(year.Value, month.Value);
                    if (specific == null)
                    {
                        return NotFound(ApiResponse<object>.Fail($"Registro não encontrado para {month}/{year}"));
                    }
                    return Ok(ApiResponse<MonthlyFinancialResponse>.Success(specific));
                }

                if (year.HasValue)
                {
                    var result = await _service.GetByYearAsync(year.Value);
                    return Ok(ApiResponse<List<MonthlyFinancialResponse>>.Success(result));
                }

                var all = await _service.GetAllAsync();
                return Ok(ApiResponse<List<MonthlyFinancialResponse>>.Success(all));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<MonthlyFinancialResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(ApiResponse<object>.Fail("Registro não encontrado"));
                }
                return Ok(ApiResponse<MonthlyFinancialResponse>.Success(result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Error($"Erro interno: {ex.Message}"));
            }
        }
    }
}
