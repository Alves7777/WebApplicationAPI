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
        [ProducesResponseType(typeof(MonthlyFinancialResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateMonthlyFinancialRequest request)
        {
            try
            {
                var result = await _service.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MonthlyFinancialResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMonthlyFinancialRequest request)
        {
            try
            {
                var result = await _service.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Registro não encontrado" });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", error = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MonthlyFinancialResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? year, [FromQuery] int? month)
        {
            try
            {
                if (year.HasValue && month.HasValue)
                {
                    var specific = await _service.GetByYearAndMonthAsync(year.Value, month.Value);
                    if (specific == null)
                    {
                        return NotFound(new { message = $"Registro não encontrado para {month}/{year}" });
                    }
                    return Ok(specific);
                }

                if (year.HasValue)
                {
                    var result = await _service.GetByYearAsync(year.Value);
                    return Ok(result);
                }

                var all = await _service.GetAllAsync();
                return Ok(all);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MonthlyFinancialResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Registro não encontrado" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", error = ex.Message });
            }
        }
    }
}
