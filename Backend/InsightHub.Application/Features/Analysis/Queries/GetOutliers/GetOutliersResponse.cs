namespace InsightHub.Application.Features.Analysis.Queries.GetOutliers;

public class GetOutliersResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public double Q1 { get; set; }

    public double Q3 { get; set; }

    public double IQR { get; set; }

    public double LowerBound { get; set; }

    public double UpperBound { get; set; }

    public int OutlierCount { get; set; }

    public List<OutlierValueResponse> Outliers { get; set; } = [];
}

public class OutlierValueResponse
{
    public int RowNumber { get; set; }

    public double Value { get; set; }
}