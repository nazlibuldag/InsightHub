using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetBarChart;

public class GetBarChartQuery : IRequest<List<GetBarChartResponse>>
{
    public Guid DatasetId { get; set; }
}