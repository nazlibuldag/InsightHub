using System;
using System.Threading.Tasks;
using InsightHub.Application.Features.Admin.Commands.ToggleUserStatus;
using InsightHub.Application.Features.Admin.Commands.UpdateUserRole;
using InsightHub.Application.Features.Admin.Queries.GetAdminStats;
using InsightHub.Application.Features.Admin.Queries.GetAdminUsers;
using InsightHub.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _mediator.Send(new GetAdminUsersQuery());
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _mediator.Send(new GetAdminStatsQuery());
        return Ok(result);
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
    {
        var success = await _mediator.Send(new UpdateUserRoleCommand(id, request.Role));
        if (!success) return NotFound("Kullanıcı bulunamadı.");
        return Ok(new { Message = "Kullanıcı rolü güncellendi." });
    }

    [HttpPut("users/{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var success = await _mediator.Send(new ToggleUserStatusCommand(id));
        if (!success) return NotFound("Kullanıcı bulunamadı.");
        return Ok(new { Message = "Kullanıcı aktiflik durumu değiştirildi." });
    }
}

public record UpdateRoleRequest(UserRole Role);
