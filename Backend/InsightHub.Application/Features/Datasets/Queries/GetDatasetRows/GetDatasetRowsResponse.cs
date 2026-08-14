namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;

public class GetDatasetRowsResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public int TotalPages { get; set; }

    public List<GetDatasetRowsItem> Rows { get; set; } = new();
}

public class GetDatasetRowsItem
{
    public int RowNumber { get; set; }

    public string Data { get; set; } = string.Empty;
}