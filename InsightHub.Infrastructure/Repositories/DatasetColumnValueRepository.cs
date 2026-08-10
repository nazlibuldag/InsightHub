using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InsightHub.Infrastructure.Repositories;

public class DatasetColumnValueRepository : IDatasetColumnValueRepository
{
    private readonly ApplicationDbContext _context;

    public DatasetColumnValueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        List<DatasetColumnValue> values,
        CancellationToken cancellationToken)
    {
        await _context.DatasetColumnValues.AddRangeAsync(values, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DatasetColumnValue>> GetByColumnIdAsync(
        Guid columnId,
        CancellationToken cancellationToken)
    {
        return await _context.DatasetColumnValues
            .Where(x => x.DatasetColumnId == columnId)
            .OrderByDescending(x => x.Count)
            .ToListAsync(cancellationToken);
    }
}