using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataQuery
    : IRequest<GetDatasetDataResponse>
{
    public Guid DatasetId { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}