using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using System.Globalization;
using InsightHub.Domain.Enums;
using CsvHelper.Configuration;

namespace InsightHub.Infrastructure.Services;

public class ColumnAnalysisService : IColumnAnalysisService
{
    public async Task<List<DatasetColumn>> AnalyzeAsync(string filePath, Guid datasetId)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord;

        if (headers == null)
            return new List<DatasetColumn>();

        // Her kolon için değerleri tutacağız
        var columnValues = headers.ToDictionary(h => h, h => new List<string>());

        // Tüm satırları oku
        while (await csv.ReadAsync())
        {
            foreach (var header in headers)
            {
                var value = csv.GetField(header);
                columnValues[header].Add(value ?? string.Empty);
            }
        }

        var columns = new List<DatasetColumn>();

        foreach (var header in headers)
        {
            var values = columnValues[header];

            columns.Add(new DatasetColumn
            {
                DatasetId = datasetId,
                ColumnName = header,
                DataType = DetectDataType(values),
                NullCount = values.Count(v => string.IsNullOrWhiteSpace(v)),
                UniqueCount = values
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct()
                    .Count()
            });
        }

        return columns;
    }

    private DataType DetectDataType(IEnumerable<string> values)
    {
        var nonEmptyValues = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        if (!nonEmptyValues.Any())
            return DataType.Unknown;

        if (nonEmptyValues.All(v => double.TryParse(v, out _)))
            return DataType.Numeric;

        if (nonEmptyValues.All(v => bool.TryParse(v, out _)))
            return DataType.Boolean;

        if (nonEmptyValues.All(v => DateTime.TryParse(v, out _)))
            return DataType.DateTime;

        return DataType.String;
    }


}