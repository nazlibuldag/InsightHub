using InsightHub.Domain.Enums;

namespace InsightHub.Application.Features.Analysis.Queries.GetColumnSummary;

public class GetColumnSummaryResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public DataType DataType { get; set; }

    public int Count { get; set; }

    public int MissingCount { get; set; }

    public int UniqueCount { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double? Mean { get; set; }

    public double? Median { get; set; }

    public double? StandardDeviation { get; set; }
}