using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InsightHub.Infrastructure.Repositories;

public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly ApplicationDbContext _context;

    public WorkspaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Workspaces
            .Include(w => w.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<List<Workspace>> GetUserWorkspacesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Workspaces
            .Include(w => w.Members)
            .Where(w => w.OwnerId == userId || w.Members.Any(m => m.UserId == userId))
            .OrderByDescending(w => w.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Workspace> AddAsync(Workspace workspace, CancellationToken cancellationToken = default)
    {
        await _context.Workspaces.AddAsync(workspace, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return workspace;
    }

    public async Task AddMemberAsync(WorkspaceMember member, CancellationToken cancellationToken = default)
    {
        await _context.WorkspaceMembers.AddAsync(member, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
