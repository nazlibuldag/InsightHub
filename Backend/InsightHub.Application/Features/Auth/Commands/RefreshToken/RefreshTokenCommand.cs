using System;
using InsightHub.Application.Features.Auth.Dtos;
using MediatR;

namespace InsightHub.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<AuthResponseDto>
{
    public string RefreshToken { get; set; } = string.Empty;
}
