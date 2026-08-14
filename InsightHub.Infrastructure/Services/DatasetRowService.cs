using ClosedXML.Excel;
using CsvHelper;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using System.Globalization;
using System.Text.Json;

namespace InsightHub.Infrastructure.Services;

public class DatasetRowService : IDatasetRowService
{
    public async Task<List<DatasetRow>> ReadRowsAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(filePath)
            .ToLowerInvariant();

        if (extension == ".csv")
        {
            return await ReadCsvRowsAsync(
                filePath,
                datasetId,
                cancellationToken);
        }

        if (extension == ".xlsx")
        {
            return await ReadExcelRowsAsync(
                filePath,
                datasetId,
                cancellationToken);
        }

        throw new Exception("Desteklenmeyen dosya formatı.");
    }

    private async Task<List<DatasetRow>> ReadCsvRowsAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken)
    {
        var rows = new List<DatasetRow>();

        using var reader = new StreamReader(filePath);

        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord;

        if (headers == null)
            return rows;

        var rowNumber = 1;

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            var rowData = new Dictionary<string, string>();

            foreach (var header in headers)
            {
                var value = csv.GetField(header);

                rowData[header] = value ?? string.Empty;
            }

            rows.Add(new DatasetRow
            {
                DatasetId = datasetId,
                RowNumber = rowNumber,
                Data = JsonSerializer.Serialize(rowData)
            });

            rowNumber++;
        }

        return rows;
    }

    private async Task<List<DatasetRow>> ReadExcelRowsAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var rows = new List<DatasetRow>();

            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheet(1);

            var firstRow = worksheet.FirstRowUsed();

            if (firstRow == null)
                return rows;

            var headers = firstRow
                .CellsUsed()
                .Select(cell => cell.GetString())
                .ToList();

            var rowNumber = 1;

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var rowData = new Dictionary<string, string>();

                for (int i = 0; i < headers.Count; i++)
                {
                    var cell = row.Cell(i + 1);

                    var value = cell.GetValue<string>();

                    rowData[headers[i]] = value;
                }

                rows.Add(new DatasetRow
                {
                    DatasetId = datasetId,
                    RowNumber = rowNumber,
                    Data = JsonSerializer.Serialize(rowData)
                });

                rowNumber++;
            }

            return rows;

        }, cancellationToken);
    }
}