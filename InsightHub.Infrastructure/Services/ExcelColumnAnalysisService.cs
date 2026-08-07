using ClosedXML.Excel;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Domain.Enums;
using System.Globalization;

namespace InsightHub.Infrastructure.Services;

public class ExcelColumnAnalysisService : IExcelColumnAnalysisService
{
    private readonly IStatisticsService _statisticsService;

    public ExcelColumnAnalysisService(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<List<DatasetColumn>> AnalyzeAsync(
        string filePath,
        Guid datasetId,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            var columns = new List<DatasetColumn>();

            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheet(1);

            var firstRow = worksheet.FirstRowUsed();

            if (firstRow == null)
                return columns;

            var headers = firstRow.CellsUsed()
                .Select(c => c.GetString())
                .ToList();

            var columnValues = headers.ToDictionary(
                h => h,
                h => new List<string>());

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                for (int i = 0; i < headers.Count; i++)
                {
                    var value = row.Cell(i + 1).Value.ToString();

                    columnValues[headers[i]].Add(value ?? string.Empty);
                }
            }

            foreach (var header in headers)
            {
                var values = columnValues[header];

                var dataType = DetectDataType(values);

                var column = new DatasetColumn
                {
                    DatasetId = datasetId,
                    ColumnName = header,
                    DataType = dataType,
                    NullCount = values.Count(v => string.IsNullOrWhiteSpace(v)),
                    UniqueCount = values
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct()
                        .Count()
                };

                if (dataType == DataType.Numeric)
                {
                    var numericValues = GetNumericValues(values);

                    if (numericValues.Any())
                    {
                        column.MinValue = _statisticsService.GetMin(numericValues);
                        column.MaxValue = _statisticsService.GetMax(numericValues);
                        column.AverageValue = _statisticsService.GetAverage(numericValues);
                        column.MedianValue = _statisticsService.GetMedian(numericValues);
                        column.StandardDeviation = _statisticsService.GetStandardDeviation(numericValues);
                    }
                }

                columns.Add(column);
            }

            return columns;

        }, cancellationToken);
    }
    private List<double> GetNumericValues(IEnumerable<string> values)
    {
        return values
            .Select(v =>
            {
                double.TryParse(
                    v.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out double result);

                return result;
            })
            .ToList();
    }

    private DataType DetectDataType(IEnumerable<string> values)
    {
        var nonEmptyValues = values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        if (!nonEmptyValues.Any())
            return DataType.Unknown;

        if (nonEmptyValues.All(v =>
            double.TryParse(
                v,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out _)))
            return DataType.Numeric;

        if (nonEmptyValues.All(v => bool.TryParse(v, out _)))
            return DataType.Boolean;

        if (nonEmptyValues.All(v => DateTime.TryParse(v, out _)))
            return DataType.DateTime;

        return DataType.String;
    }
}