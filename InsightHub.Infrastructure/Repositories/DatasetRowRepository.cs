using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InsightHub.Infrastructure.Repositories;

public class DatasetRowRepository : IDatasetRowRepository
{
    private readonly ApplicationDbContext _context;

    public DatasetRowRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        List<DatasetRow> rows,
        CancellationToken cancellationToken)
    {
        await _context.DatasetRows.AddRangeAsync(
            rows,
            cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DatasetRow>> GetByDatasetIdAsync(
        Guid datasetId,
        CancellationToken cancellationToken)
    {
        return await _context.DatasetRows
            .Where(x => x.DatasetId == datasetId)
            .OrderBy(x => x.RowNumber)
            .ToListAsync(cancellationToken);
    }
}