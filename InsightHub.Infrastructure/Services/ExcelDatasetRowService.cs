using ClosedXML.Excel;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using System.Text.Json;

namespace InsightHub.Infrastructure.Services;

public class ExcelDatasetRowService : IExcelDatasetRowService
{
    public async Task<List<DatasetRow>> ReadRowsAsync(
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

            var headers = firstRow.CellsUsed()
                .Select(cell => cell.GetString())
                .ToList();

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var data = new Dictionary<string, string>();

                for (int i = 0; i < headers.Count; i++)
                {
                    data[headers[i]] =
                        row.Cell(i + 1).Value.ToString();
                }

                rows.Add(new DatasetRow
                {
                    DatasetId = datasetId,
                    RowNumber = rows.Count + 1,
                    Data = JsonSerializer.Serialize(data)
                });
            }

            return rows;

        }, cancellationToken);
    }
}