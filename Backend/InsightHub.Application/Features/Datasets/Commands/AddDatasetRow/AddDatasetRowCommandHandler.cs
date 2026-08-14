using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.AddDatasetRow;

public class AddDatasetRowCommandHandler
    : IRequestHandler<AddDatasetRowCommand, bool>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public AddDatasetRowCommandHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<bool> Handle(
        AddDatasetRowCommand request,
        CancellationToken cancellationToken)
    {
        var row = new DatasetRow
        {
            Id = Guid.NewGuid(),
            DatasetId = request.DatasetId,
            RowNumber = request.RowNumber,
            Data = request.Data
        };

        await _datasetRowRepository.AddRangeAsync(
            new List<DatasetRow> { row },
            cancellationToken);

        return true;
    }
}