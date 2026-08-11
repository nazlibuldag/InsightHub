using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.SearchDataset;

public class SearchDatasetQuery
    : IRequest<SearchDatasetResponse>
{
    public Guid DatasetId { get; set; }

    public string? SearchTerm { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}