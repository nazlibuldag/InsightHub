using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IExcelDatasetRowService
{
    Task<List<DatasetRow>> ReadRowsAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken);
}