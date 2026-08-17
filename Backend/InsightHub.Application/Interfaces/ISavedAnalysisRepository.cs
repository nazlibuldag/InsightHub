using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface ISavedAnalysisRepository
{
    Task<SavedAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SavedAnalysis?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SavedAnalysis>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<SavedAnalysis>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SavedAnalysis> AddAsync(SavedAnalysis savedAnalysis, CancellationToken cancellationToken = default);
    Task DeleteAsync(SavedAnalysis savedAnalysis, CancellationToken cancellationToken = default);
}
