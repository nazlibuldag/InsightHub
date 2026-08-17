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

public class SavedAnalysisRepository : ISavedAnalysisRepository
{
    private readonly ApplicationDbContext _context;

    public SavedAnalysisRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SavedAnalysis?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses
            .Include(x => x.Dataset)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SavedAnalysis?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses
            .Include(x => x.Dataset)
                .ThenInclude(d => d!.Columns)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<SavedAnalysis>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses
            .Include(x => x.Dataset)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SavedAnalysis>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SavedAnalyses
            .Include(x => x.Dataset)
            .Include(x => x.User)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<SavedAnalysis> AddAsync(SavedAnalysis savedAnalysis, CancellationToken cancellationToken = default)
    {
        await _context.SavedAnalyses.AddAsync(savedAnalysis, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return savedAnalysis;
    }

    public async Task DeleteAsync(SavedAnalysis savedAnalysis, CancellationToken cancellationToken = default)
    {
        _context.SavedAnalyses.Remove(savedAnalysis);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
