using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Workspace>> GetUserWorkspacesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken = default);
    Task AddMemberAsync(WorkspaceMember member, CancellationToken cancellationToken = default);
}
