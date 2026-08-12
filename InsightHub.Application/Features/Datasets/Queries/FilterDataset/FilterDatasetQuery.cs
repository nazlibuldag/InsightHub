using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.FilterDataset;

public class FilterDatasetQuery
    : IRequest<List<FilterDatasetResponse>>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}