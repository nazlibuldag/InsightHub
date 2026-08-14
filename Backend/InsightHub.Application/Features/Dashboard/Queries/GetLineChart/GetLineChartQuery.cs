using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetLineChart;

public class GetLineChartQuery : IRequest<List<GetLineChartResponse>>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;
}