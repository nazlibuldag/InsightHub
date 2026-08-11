namespace InsightHub.Application.Features.Datasets.Queries.SearchDataset;

public class SearchDatasetResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public int TotalPages { get; set; }

    public List<SearchDatasetRowResponse> Rows { get; set; } = new();
}

public class SearchDatasetRowResponse
{
    public int RowNumber { get; set; }

    public string Data { get; set; } = string.Empty;
}