using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IDatasetRowRepository
{
    Task AddRangeAsync(
        List<DatasetRow> rows,
        CancellationToken cancellationToken);

    Task<List<DatasetRow>> GetByDatasetIdAsync(
        Guid datasetId,
        CancellationToken cancellationToken);
}