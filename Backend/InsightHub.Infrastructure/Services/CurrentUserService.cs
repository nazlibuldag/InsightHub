using System;
using System.Security.Claims;
using InsightHub.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InsightHub.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user?.FindFirst("sub")?.Value;

            if (Guid.TryParse(userIdStr, out var id))
            {
                return id;
            }

            return null;
        }
    }

    public string? UserEmail =>
        _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin =>
        _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
}
