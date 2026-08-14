namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataResponse
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalRows { get; set; }

    public int TotalPages { get; set; }

    public List<DatasetRowResponse> Rows { get; set; } = new();
}

public class DatasetRowResponse
{
    public int RowNumber { get; set; }

    public string Data { get; set; } = string.Empty;
}