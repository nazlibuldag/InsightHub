using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UpdateDataset;

public class UpdateDatasetCommandHandler
    : IRequestHandler<UpdateDatasetCommand, bool>
{
    private readonly IDatasetRepository _datasetRepository;

    public UpdateDatasetCommandHandler(
        IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<bool> Handle(
        UpdateDatasetCommand request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            return false;

        dataset.Name = request.Name;
        dataset.Description = request.Description;

        await _datasetRepository.UpdateAsync(
            dataset,
            cancellationToken);

        return true;
    }
}