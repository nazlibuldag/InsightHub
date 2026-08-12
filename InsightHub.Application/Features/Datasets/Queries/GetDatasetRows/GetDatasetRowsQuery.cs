using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;

public class GetDatasetRowsQuery
    : IRequest<GetDatasetRowsResponse>
{
    public Guid DatasetId { get; set; }

    public string? SearchTerm { get; set; }

    public string? FilterColumn { get; set; }

    public string? FilterOperator { get; set; }

    public string? FilterValue { get; set; }

    public string? SortColumn { get; set; }

    public string SortOrder { get; set; } = "asc";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}