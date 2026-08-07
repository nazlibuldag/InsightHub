using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace InsightHub.Application.Interfaces;

public interface IExcelReaderService
{
    Task<(int TotalRows, int TotalColumns)> ReadExcelInfoAsync(
        string filePath,
        CancellationToken cancellationToken);
}