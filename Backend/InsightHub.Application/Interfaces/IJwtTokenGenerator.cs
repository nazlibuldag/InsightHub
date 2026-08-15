using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
