using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UpdateDataset;

public class UpdateDatasetCommand : IRequest<bool>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}