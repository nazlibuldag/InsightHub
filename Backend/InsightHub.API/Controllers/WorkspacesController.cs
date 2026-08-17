using System;
using System.Security.Claims;
using System.Threading.Tasks;
using InsightHub.Application.Features.Workspaces.Commands.CreateWorkspace;
using InsightHub.Application.Features.Workspaces.Queries.GetUserWorkspaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkspacesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspacesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserWorkspaces()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetUserWorkspacesQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest request)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new CreateWorkspaceCommand(
            userId,
            request.Name,
            request.Description
        ));
        return Ok(result);
    }
}

public record CreateWorkspaceRequest(
    string Name,
    string Description
);
