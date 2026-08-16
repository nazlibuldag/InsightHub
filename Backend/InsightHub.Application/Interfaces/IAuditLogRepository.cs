using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog log, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetRecentLogsAsync(int count = 50, CancellationToken cancellationToken = default);
    Task<List<AuditLog>> GetUserLogsAsync(Guid userId, int count = 50, CancellationToken cancellationToken = default);
}
