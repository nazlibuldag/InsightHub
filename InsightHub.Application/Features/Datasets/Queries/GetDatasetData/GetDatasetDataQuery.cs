using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataQuery
    : IRequest<List<GetDatasetDataResponse>>
{
    public Guid DatasetId { get; set; }
}