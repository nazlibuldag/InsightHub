using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetCorrelationMatrix;

public class GetCorrelationMatrixQuery
    : IRequest<GetCorrelationMatrixResponse>
{
    public Guid DatasetId { get; set; }
}