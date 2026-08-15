using System.Threading.Tasks;
using InsightHub.Application.Features.Auth.Commands.LoginUser;
using InsightHub.Application.Features.Auth.Commands.RegisterUser;
using InsightHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;

    public AuthController(
        IMediator mediator,
        ICurrentUserService currentUserService,
        IUserRepository userRepository)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] InsightHub.Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = _currentUserService.UserId;
        if (!userId.HasValue) return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role
        });
    }
}
