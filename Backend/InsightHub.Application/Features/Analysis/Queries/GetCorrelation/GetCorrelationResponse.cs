namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelation;

public class GetCorrelationResponse
{
    public string Column1 { get; set; } = string.Empty;

    public string Column2 { get; set; } = string.Empty;

    public double Correlation { get; set; }
}