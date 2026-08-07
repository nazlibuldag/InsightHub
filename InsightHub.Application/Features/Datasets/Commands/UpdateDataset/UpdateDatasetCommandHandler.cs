using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UpdateDataset;

public class UpdateDatasetCommandHandler : IRequestHandler<UpdateDatasetCommand>
{
    private readonly IDatasetRepository _datasetRepository;

    public UpdateDatasetCommandHandler(IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task Handle(
        UpdateDatasetCommand request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        dataset.Name = request.Name;
        dataset.Description = request.Description;

        await _datasetRepository.UpdateAsync(
            dataset,
            cancellationToken);
    }
}