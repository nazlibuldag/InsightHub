using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.SortDataset;

public class SortDatasetQuery
    : IRequest<List<SortDatasetResponse>>
{
    public Guid DatasetId { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public string SortOrder { get; set; } = "asc";
}