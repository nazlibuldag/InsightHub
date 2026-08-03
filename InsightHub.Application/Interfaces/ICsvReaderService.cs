using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsightHub.Application.Interfaces;

public interface ICsvReaderService
{
    Task<(int TotalRows, int TotalColumns)> ReadCsvInfoAsync(
        string filePath,
        CancellationToken cancellationToken);
}