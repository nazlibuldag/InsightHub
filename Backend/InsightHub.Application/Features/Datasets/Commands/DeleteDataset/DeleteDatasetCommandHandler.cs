using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.DeleteDataset;

public class DeleteDatasetCommandHandler
    : IRequestHandler<DeleteDatasetCommand, bool>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetColumnRepository _datasetColumnRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IDatasetColumnValueRepository _datasetColumnValueRepository;

    public DeleteDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IDatasetColumnRepository datasetColumnRepository,
        IDatasetRowRepository datasetRowRepository,
        IDatasetColumnValueRepository datasetColumnValueRepository)
    {
        _datasetRepository = datasetRepository;
        _datasetColumnRepository = datasetColumnRepository;
        _datasetRowRepository = datasetRowRepository;
        _datasetColumnValueRepository = datasetColumnValueRepository;
    }

    public async Task<bool> Handle(
        DeleteDatasetCommand request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            return false;

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.Id,
            cancellationToken);

        if (rows.Any())
        {
            await _datasetRowRepository.DeleteRangeAsync(
                rows,
                cancellationToken);
        }

        var columns = await _datasetColumnRepository.GetByDatasetIdAsync(
            request.Id,
            cancellationToken);

        foreach (var column in columns)
        {
            var values =
                await _datasetColumnValueRepository.GetByColumnIdAsync(
                    column.Id,
                    cancellationToken);

            if (values.Any())
            {
                await _datasetColumnValueRepository.DeleteRangeAsync(
                    values,
                    cancellationToken);
            }
        }

        if (columns.Any())
        {
            await _datasetColumnRepository.DeleteRangeAsync(
                columns,
                cancellationToken);
        }

        await _datasetRepository.DeleteAsync(
            dataset,
            cancellationToken);

        return true;
    }
}