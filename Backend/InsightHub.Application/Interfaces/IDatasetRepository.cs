using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IDatasetRepository
{
    Task AddAsync(Dataset dataset, CancellationToken cancellationToken);

    Task<Dataset?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<List<Dataset>> GetAllAsync(CancellationToken cancellationToken);

    Task<List<Dataset>> GetAllByUserIdAsync(Guid? userId, CancellationToken cancellationToken);

    Task<Dataset?> GetByIdWithColumnsAsync( Guid id,CancellationToken cancellationToken);

    Task UpdateAsync(Dataset dataset, CancellationToken cancellationToken);

    Task DeleteAsync(Dataset dataset, CancellationToken cancellationToken);

}