using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDescriptiveStatistics;

public class GetDescriptiveStatisticsQuery : IRequest<GetDescriptiveStatisticsResponse?>
{
    public Guid DatasetId { get; set; }
    public string ColumnName { get; set; } = string.Empty;
}