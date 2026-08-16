using System.Threading.Tasks;
using InsightHub.Application.Features.AuditLogs.Queries.GetRecentAuditLogs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuditLogsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetRecentLogs([FromQuery] int count = 50)
    {
        var result = await _mediator.Send(new GetRecentAuditLogsQuery(count));
        return Ok(result);
    }
}
