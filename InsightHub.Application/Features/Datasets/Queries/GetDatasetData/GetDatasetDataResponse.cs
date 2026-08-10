namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public List<string> Values { get; set; } = new();
}