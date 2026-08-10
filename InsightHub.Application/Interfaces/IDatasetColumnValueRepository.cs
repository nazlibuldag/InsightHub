using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IDatasetColumnValueRepository
{
    Task AddRangeAsync(
        List<DatasetColumnValue> values,
        CancellationToken cancellationToken);

    Task<List<DatasetColumnValue>> GetByColumnIdAsync(
        Guid columnId,
        CancellationToken cancellationToken);
}