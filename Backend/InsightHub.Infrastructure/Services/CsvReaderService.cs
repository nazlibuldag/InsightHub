using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using InsightHub.Application.Interfaces;
using System.Globalization;

namespace InsightHub.Infrastructure.Services;

public class CsvReaderService : ICsvReaderService
{
    public async Task<(int TotalRows, int TotalColumns)> ReadCsvInfoAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(filePath);

        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<dynamic>().ToList();

        var totalRows = records.Count;

        var totalColumns = 0;

        if (records.Any())
        {
            var firstRow = (IDictionary<string, object>)records.First();

            totalColumns = firstRow.Keys.Count;
        }

        return (totalRows, totalColumns);
    }
}