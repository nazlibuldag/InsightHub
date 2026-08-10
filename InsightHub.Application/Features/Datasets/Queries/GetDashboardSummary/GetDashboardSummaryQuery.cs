using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQuery : IRequest<GetDashboardSummaryResponse>
{
    public Guid DatasetId { get; set; }
}