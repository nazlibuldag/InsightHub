using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.ExportDataset;

public class ExportDatasetQueryHandler : IRequestHandler<ExportDatasetQuery, ExportDatasetResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;

    public ExportDatasetQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRepository = datasetRepository;
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<ExportDatasetResponse> Handle(ExportDatasetQuery request, CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new KeyNotFoundException("Dataset bulunamadı.");
        }

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(request.DatasetId, cancellationToken);
        var columns = dataset.Columns.OrderBy(c => c.ColumnName).Select(c => c.ColumnName).ToList();

        if (!columns.Any() && rows.Any())
        {
            using var doc = JsonDocument.Parse(rows.First().Data);
            columns = doc.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        }

        bool isExcel = request.Format.Equals("excel", StringComparison.OrdinalIgnoreCase) ||
                       request.Format.Equals("xlsx", StringComparison.OrdinalIgnoreCase);

        if (isExcel)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Dataset");

            // Header row
            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var cell = worksheet.Cell(1, colIdx + 1);
                cell.Value = columns[colIdx];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#635BFF");
                cell.Style.Font.FontColor = XLColor.White;
            }

            // Data rows
            for (int rowIdx = 0; rowIdx < rows.Count; rowIdx++)
            {
                using var doc = JsonDocument.Parse(rows[rowIdx].Data);
                for (int colIdx = 0; colIdx < columns.Count; colIdx++)
                {
                    var colName = columns[colIdx];
                    if (doc.RootElement.TryGetProperty(colName, out var prop))
                    {
                        worksheet.Cell(rowIdx + 2, colIdx + 1).Value = prop.ToString();
                    }
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return new ExportDatasetResponse
            {
                FileBytes = stream.ToArray(),
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileName = $"{dataset.Name.Replace(" ", "_")}_export.xlsx"
            };
        }
        else
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(",", columns.Select(c => $"\"{c}\"")));

            foreach (var row in rows)
            {
                using var doc = JsonDocument.Parse(row.Data);
                var values = columns.Select(colName =>
                {
                    if (doc.RootElement.TryGetProperty(colName, out var prop))
                    {
                        return $"\"{prop.ToString().Replace("\"", "\"\"")}\"";
                    }
                    return "\"\"";
                });
                sb.AppendLine(string.Join(",", values));
            }

            return new ExportDatasetResponse
            {
                FileBytes = Encoding.UTF8.GetBytes(sb.ToString()),
                ContentType = "text/csv",
                FileName = $"{dataset.Name.Replace(" ", "_")}_export.csv"
            };
        }
    }
}
