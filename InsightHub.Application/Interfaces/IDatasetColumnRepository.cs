using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IDatasetColumnRepository
{
    Task AddRangeAsync(List<DatasetColumn> columns, CancellationToken cancellationToken);

    Task<List<DatasetColumn>> GetByDatasetIdAsync(
    Guid datasetId,
    CancellationToken cancellationToken);

    Task DeleteRangeAsync(
        List<DatasetColumn> columns,
        CancellationToken cancellationToken);

}