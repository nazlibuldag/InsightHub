using System;
using InsightHub.Domain.Enums;

namespace InsightHub.Application.Features.Auth.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public UserDto User { get; set; } = null!;
}

public class UserDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}
