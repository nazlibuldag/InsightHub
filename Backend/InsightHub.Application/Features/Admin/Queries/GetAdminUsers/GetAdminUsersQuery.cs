using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Admin.Queries.GetAdminUsers;

public record AdminUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    bool IsActive,
    int DatasetCount,
    int SavedAnalysisCount,
    DateTime CreatedDate
);

public record GetAdminUsersQuery() : IRequest<List<AdminUserDto>>;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, List<AdminUserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAdminUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<AdminUserDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllWithStatsAsync(cancellationToken);

        return users.Select(u => new AdminUserDto(
            u.Id,
            u.FirstName,
            u.LastName,
            u.Email,
            u.Role,
            u.IsActive,
            u.Datasets?.Count ?? 0,
            u.SavedAnalyses?.Count ?? 0,
            u.CreatedDate
        )).ToList();
    }
}
