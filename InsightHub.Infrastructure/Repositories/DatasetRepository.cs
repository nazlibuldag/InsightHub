using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InsightHub.Infrastructure.Repositories;

public class DatasetRepository : IDatasetRepository
{
    private readonly ApplicationDbContext _context;

    public DatasetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Dataset dataset, CancellationToken cancellationToken)
    {
        await _context.Datasets.AddAsync(dataset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Dataset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Datasets
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<List<Dataset>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Datasets.ToListAsync(cancellationToken);
    }

    public async Task<Dataset?> GetByIdWithColumnsAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Datasets
            .Include(x => x.Columns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(
    Dataset dataset,
    CancellationToken cancellationToken)
    {
        _context.Datasets.Update(dataset);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
    Dataset dataset,
    CancellationToken cancellationToken)
    {
        _context.Datasets.Remove(dataset);

        await _context.SaveChangesAsync(cancellationToken);
    }
}