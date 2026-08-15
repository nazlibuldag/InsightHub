using MediatR;
using InsightHub.Application.Features.Auth.Dtos;
using InsightHub.Domain.Enums;

namespace InsightHub.Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<AuthResponseDto>
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Analyst;
}
