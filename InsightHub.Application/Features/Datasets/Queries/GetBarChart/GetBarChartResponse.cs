namespace InsightHub.Application.Features.Dashboard.Queries.GetBarChart;

public class GetBarChartResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public double Average { get; set; }
}