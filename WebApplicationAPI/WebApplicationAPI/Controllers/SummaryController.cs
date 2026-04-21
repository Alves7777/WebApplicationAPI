using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebApplicationAPI.Queries.Summary;
using WebApplicationAPI.Extensions;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SummaryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SummaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? month, [FromQuery] int? year, [FromQuery] decimal? salary, [FromQuery] decimal? reserve)
        {
            var userId = this.GetUserId(); // ? Pega do token JWT

            var query = new GetFinancialSummaryQuery
            {
                UserId = userId, // ? Adiciona userId na query
                Month = month ?? DateTime.Now.Month,
                Year = year ?? DateTime.Now.Year,
                Salary = salary ?? 0,
                Reserve = reserve ?? 0
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}