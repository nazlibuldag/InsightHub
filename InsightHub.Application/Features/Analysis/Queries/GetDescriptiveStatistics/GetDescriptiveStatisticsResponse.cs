namespace InsightHub.Application.Features.Analysis.Queries.GetDescriptiveStatistics;

public class GetDescriptiveStatisticsResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public int Count { get; set; }

    public double? Mean { get; set; }

    public double? Median { get; set; }

    public double? Mode { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public double? Range { get; set; }

    public double? Q1 { get; set; }

    public double? Q3 { get; set; }

    public double? IQR { get; set; }

    public double? Variance { get; set; }

    public double? StandardDeviation { get; set; }
}