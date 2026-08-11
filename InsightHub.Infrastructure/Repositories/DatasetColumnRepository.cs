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

public class DatasetColumnRepository : IDatasetColumnRepository
{
    private readonly ApplicationDbContext _context;

    public DatasetColumnRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(List<DatasetColumn> columns, CancellationToken cancellationToken)
    {
        await _context.DatasetColumns.AddRangeAsync(columns, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<DatasetColumn>> GetByDatasetIdAsync(
    Guid datasetId,
    CancellationToken cancellationToken)
    {
        return await _context.DatasetColumns
            .Where(x => x.DatasetId == datasetId)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(
        List<DatasetColumn> columns,
        CancellationToken cancellationToken)
    {
        _context.DatasetColumns.RemoveRange(columns);

        await _context.SaveChangesAsync(cancellationToken);
    }
}