using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Admin.Queries.GetAdminStats;

public record AdminStatsDto(
    int TotalUsers,
    int TotalDatasets,
    long TotalRows,
    int TotalSavedAnalyses,
    int ActiveUsersCount,
    int AdminUsersCount
);

public record GetAdminStatsQuery() : IRequest<AdminStatsDto>;

public class GetAdminStatsQueryHandler : IRequestHandler<GetAdminStatsQuery, AdminStatsDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public GetAdminStatsQueryHandler(
        IUserRepository userRepository,
        IDatasetRepository datasetRepository,
        ISavedAnalysisRepository savedAnalysisRepository)
    {
        _userRepository = userRepository;
        _datasetRepository = datasetRepository;
        _savedAnalysisRepository = savedAnalysisRepository;
    }

    public async Task<AdminStatsDto> Handle(GetAdminStatsQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllWithStatsAsync(cancellationToken);
        var datasets = await _datasetRepository.GetAllAsync(cancellationToken);
        var savedAnalyses = await _savedAnalysisRepository.GetAllAsync(cancellationToken);

        int totalUsers = users.Count;
        int activeUsers = users.Count(u => u.IsActive);
        int adminUsers = users.Count(u => u.Role == Domain.Enums.UserRole.Admin);
        int totalDatasets = datasets.Count;
        long totalRows = datasets.Sum(d => (long)d.TotalRows);
        int totalSavedAnalyses = savedAnalyses.Count;

        return new AdminStatsDto(
            totalUsers,
            totalDatasets,
            totalRows,
            totalSavedAnalyses,
            activeUsers,
            adminUsers
        );
    }
}
