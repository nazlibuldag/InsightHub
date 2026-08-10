using InsightHub.Application.Features.Datasets.Queries.GetPieChart;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetPieChart;

public class GetPieChartQuery : IRequest<List<GetPieChartResponse>>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;
}