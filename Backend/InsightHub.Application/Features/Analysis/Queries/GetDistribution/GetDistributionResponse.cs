namespace InsightHub.Application.Features.Analysis.Queries.GetDistribution;

public class GetDistributionResponse
{
    public string ColumnName { get; set; } = string.Empty;

    public double MinValue { get; set; }

    public double MaxValue { get; set; }

    public int BinCount { get; set; }

    public List<DistributionBinResponse> Bins { get; set; } = [];
}

public class DistributionBinResponse
{
    public double From { get; set; }

    public double To { get; set; }

    public int Count { get; set; }
}