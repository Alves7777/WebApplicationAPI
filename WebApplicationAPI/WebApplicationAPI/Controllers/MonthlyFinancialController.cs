using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v1/monthly-financial")]
    [Authorize]
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
                var userId = this.GetUserId(); // ? Pega do token JWT
                var result = await _service.CreateAsync(userId, request);
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
                var userId = this.GetUserId(); // ? Pega do token JWT
                var result = await _service.UpdateAsync(id, userId, request);
                return Ok(ApiResponse<MonthlyFinancialResponse>.Success(result, "Controle mensal atualizado"));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ApiResponse<object>.Fail(ex.Message));
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
                var userId = this.GetUserId(); // ? Pega do token JWT
                var result = await _service.DeleteAsync(id, userId);
                if (!result)
                {
                    return NotFound(ApiResponse<object>.Fail("Registro não encontrado"));
                }
                return Ok(ApiResponse.Success("Registro deletado com sucesso"));
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
        [ProducesResponseType(typeof(ApiResponse<List<MonthlyFinancialResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? year, [FromQuery] int? month)
        {
            try
            {
                var userId = this.GetUserId(); // ? Pega do token JWT

                if (year.HasValue && month.HasValue)
                {
                    var specific = await _service.GetByYearAndMonthAsync(userId, year.Value, month.Value);
                    if (specific == null)
                    {
                        return NotFound(ApiResponse<object>.Fail($"Registro não encontrado para {month}/{year}"));
                    }
                    return Ok(ApiResponse<MonthlyFinancialResponse>.Success(specific));
                }

                if (year.HasValue)
                {
                    var result = await _service.GetByYearAsync(userId, year.Value);
                    return Ok(ApiResponse<List<MonthlyFinancialResponse>>.Success(result));
                }

                var all = await _service.GetAllAsync(userId);
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
                var userId = this.GetUserId(); // ? Pega do token JWT
                var result = await _service.GetByIdAsync(id, userId);
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
