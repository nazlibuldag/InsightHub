namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelationMatrix;

public class GetCorrelationMatrixResponse
{
    public List<string> Columns { get; set; } = [];

    public List<List<double>> Matrix { get; set; } = [];
}