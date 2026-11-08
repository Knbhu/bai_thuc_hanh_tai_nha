using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PolicyService.Api.Commands;
using static System.String;

namespace PolicyService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OfferController : ControllerBase
{
    private readonly IMediator bus;

    public OfferController(IMediator bus)
    {
        this.bus = bus;
    }

    // POST api/values
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateOfferCommand cmd, [FromHeader] string AgentLogin)
    {
        var result = IsNullOrWhiteSpace(AgentLogin)
            ? await bus.Send(cmd)
            : await bus.Send(new CreateOfferByAgentCommand(AgentLogin, cmd));
        return new JsonResult(result);
    }

    [HttpGet("{offerNumber}")]
    public async Task<ActionResult> Get(string offerNumber)
    {
        var result = await bus.Send(new GetOfferQuery { OfferNumber = offerNumber });
        return result == null ? NotFound() : new JsonResult(result);
    }
}