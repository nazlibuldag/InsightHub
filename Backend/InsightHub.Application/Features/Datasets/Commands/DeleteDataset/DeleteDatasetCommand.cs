using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.DeleteDataset;

public class DeleteDatasetCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}