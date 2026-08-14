using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetColumnSummary;

public class GetColumnSummaryQuery : IRequest<GetColumnSummaryResponse?>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;
}