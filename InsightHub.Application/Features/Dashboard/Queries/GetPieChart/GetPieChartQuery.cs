using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetPieChart;

public class GetPieChartQuery : IRequest<List<GetPieChartResponse>>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;
}