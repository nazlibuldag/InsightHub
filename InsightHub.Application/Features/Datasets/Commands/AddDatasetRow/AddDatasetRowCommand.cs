using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.AddDatasetRow;

public class AddDatasetRowCommand : IRequest<bool>
{
    public Guid DatasetId { get; set; }

    public int RowNumber { get; set; }

    public string Data { get; set; } = string.Empty;
}