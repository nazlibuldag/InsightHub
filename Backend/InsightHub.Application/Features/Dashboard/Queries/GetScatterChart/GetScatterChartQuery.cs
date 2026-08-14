using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetScatterChart;

public class GetScatterChartQuery
    : IRequest<List<GetScatterChartResponse>>
{
    public Guid DatasetId { get; set; }

    public string XColumnName { get; set; } = string.Empty;

    public string YColumnName { get; set; } = string.Empty;
}