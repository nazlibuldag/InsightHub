using System;

namespace InsightHub.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? UserEmail { get; }

    bool IsAuthenticated { get; }

    bool IsAdmin { get; }
}
