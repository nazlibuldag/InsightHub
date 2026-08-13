using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.DeleteDatasetRow;

public class DeleteDatasetRowCommand : IRequest<bool>
{
    public Guid DatasetId { get; set; }

    public int RowNumber { get; set; }
}