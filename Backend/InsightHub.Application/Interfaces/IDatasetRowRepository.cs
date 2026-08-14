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

    Task<DatasetRow?> GetByDatasetIdAndRowNumberAsync(
    Guid datasetId,
    int rowNumber,
    CancellationToken cancellationToken);

    Task DeleteRangeAsync(
    List<DatasetRow> rows,
    CancellationToken cancellationToken);

    Task<List<DatasetRow>> SearchAsync(
    Guid datasetId,
    string? searchTerm,
    CancellationToken cancellationToken);

    Task UpdateAsync(
    DatasetRow row,
    CancellationToken cancellationToken);

    Task DeleteAsync(
    DatasetRow row,
    CancellationToken cancellationToken);
}