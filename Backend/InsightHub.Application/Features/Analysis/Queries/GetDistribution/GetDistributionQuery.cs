using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDistribution;

public class GetDistributionQuery
    : IRequest<GetDistributionResponse>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public int BinCount { get; set; } = 10;
}