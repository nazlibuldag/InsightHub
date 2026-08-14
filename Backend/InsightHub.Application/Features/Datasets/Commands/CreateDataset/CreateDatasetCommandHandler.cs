using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.CreateDataset;

public class CreateDatasetCommandHandler : IRequestHandler<CreateDatasetCommand, Guid>
{
    private readonly IDatasetRepository _datasetRepository;

    public CreateDatasetCommandHandler(IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<Guid> Handle(
        CreateDatasetCommand request,
        CancellationToken cancellationToken)
    {
        var dataset = new Dataset
        {
            Name = request.Name,
            Description = request.Description
        };

        await _datasetRepository.AddAsync(dataset, cancellationToken);

        return dataset.Id;
    }
}