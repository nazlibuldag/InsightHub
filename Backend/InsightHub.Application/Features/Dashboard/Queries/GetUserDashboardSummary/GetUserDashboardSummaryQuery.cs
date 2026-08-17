using System;
using System.Collections.Generic;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetUserDashboardSummary;

public record GetUserDashboardSummaryQuery(Guid UserId, bool IsAdmin) : IRequest<UserDashboardSummaryDto>;

public class UserDashboardSummaryDto
{
    public int TotalDatasets { get; set; }
    public int TotalSavedAnalyses { get; set; }
    public long TotalRows { get; set; }
    public string? RecentDatasetName { get; set; }
    public DateTime? RecentDatasetUploadedAt { get; set; }
    public List<UserRecentDatasetDto> RecentDatasets { get; set; } = new();
    public List<UserRecentAnalysisDto> RecentAnalyses { get; set; } = new();
}

public class UserRecentDatasetDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalRows { get; set; }
    public int TotalColumns { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class UserRecentAnalysisDto
{
    public Guid Id { get; set; }
    public Guid DatasetId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DatasetName { get; set; } = string.Empty;
    public string AnalysisType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}
