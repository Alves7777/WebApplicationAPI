using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAPI.Queries.Summary;

namespace WebApplicationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SummaryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int month, [FromQuery] int year, [FromQuery] decimal salary, [FromQuery] decimal reserve)
        {
            var query = new GetFinancialSummaryQuery
            {
                Month = month,
                Year = year,
                Salary = salary,
                Reserve = reserve
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}