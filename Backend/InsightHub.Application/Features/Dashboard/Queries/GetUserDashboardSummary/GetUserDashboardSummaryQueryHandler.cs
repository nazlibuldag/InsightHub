using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetUserDashboardSummary;

public class GetUserDashboardSummaryQueryHandler : IRequestHandler<GetUserDashboardSummaryQuery, UserDashboardSummaryDto>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public GetUserDashboardSummaryQueryHandler(
        IDatasetRepository datasetRepository,
        ISavedAnalysisRepository savedAnalysisRepository)
    {
        _datasetRepository = datasetRepository;
        _savedAnalysisRepository = savedAnalysisRepository;
    }

    public async Task<UserDashboardSummaryDto> Handle(GetUserDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var allDatasets = await _datasetRepository.GetAllAsync(cancellationToken);
        var userDatasets = request.IsAdmin
            ? allDatasets.ToList()
            : allDatasets.Where(d => d.UserId == request.UserId).ToList();

        var userAnalyses = await _savedAnalysisRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (request.IsAdmin)
        {
            // If admin, they see all saved analyses
            userAnalyses = await _savedAnalysisRepository.GetAllAsync(cancellationToken);
        }

        var sortedDatasets = userDatasets.OrderByDescending(d => d.UploadedAt).ToList();
        var sortedAnalyses = userAnalyses.OrderByDescending(a => a.CreatedDate).ToList();

        var latestDataset = sortedDatasets.FirstOrDefault();

        return new UserDashboardSummaryDto
        {
            TotalDatasets = userDatasets.Count,
            TotalSavedAnalyses = userAnalyses.Count,
            TotalRows = userDatasets.Sum(d => (long)d.TotalRows),
            RecentDatasetName = latestDataset?.Name,
            RecentDatasetUploadedAt = latestDataset?.UploadedAt,
            RecentDatasets = sortedDatasets.Take(5).Select(d => new UserRecentDatasetDto
            {
                Id = d.Id,
                Name = d.Name,
                TotalRows = d.TotalRows,
                TotalColumns = d.TotalColumns,
                UploadedAt = d.UploadedAt
            }).ToList(),
            RecentAnalyses = sortedAnalyses.Take(5).Select(a => new UserRecentAnalysisDto
            {
                Id = a.Id,
                DatasetId = a.DatasetId,
                Title = a.Title,
                DatasetName = a.Dataset?.Name ?? "Veri Seti",
                AnalysisType = a.AnalysisType,
                CreatedDate = a.CreatedDate
            }).ToList()
        };
    }
}
