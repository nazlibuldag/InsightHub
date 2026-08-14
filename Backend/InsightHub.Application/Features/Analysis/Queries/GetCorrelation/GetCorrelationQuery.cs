using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelation;

public class GetCorrelationQuery : IRequest<GetCorrelationResponse>
{
    public Guid DatasetId { get; set; }

    public string Column1 { get; set; } = string.Empty;

    public string Column2 { get; set; } = string.Empty;
}