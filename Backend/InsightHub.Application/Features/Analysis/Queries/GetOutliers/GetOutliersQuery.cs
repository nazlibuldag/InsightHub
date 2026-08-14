using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetOutliers;

public class GetOutliersQuery : IRequest<GetOutliersResponse>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;
}