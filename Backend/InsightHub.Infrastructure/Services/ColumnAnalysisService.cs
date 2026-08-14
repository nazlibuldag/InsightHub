using CsvHelper;
using InsightHub.Application.Common;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using InsightHub.Domain.Enums;
using System.Globalization;

namespace InsightHub.Infrastructure.Services;

public class ColumnAnalysisService : IColumnAnalysisService
{
    private readonly IStatisticsService _statisticsService;

    public ColumnAnalysisService(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public async Task<ColumnAnalysisResult> AnalyzeAsync(
        string filePath,
        Guid datasetId)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        await csv.ReadAsync();
        csv.ReadHeader();

        var headers = csv.HeaderRecord;

        if (headers == null)
            return new ColumnAnalysisResult();

        // Her kolonun değerlerini tutuyoruz
        var columnValues = headers.ToDictionary(
            h => h,
            h => new List<string>());

        // CSV içerisindeki tüm satırları oku
        while (await csv.ReadAsync())
        {
            foreach (var header in headers)
            {
                var value = csv.GetField(header);

                columnValues[header].Add(
                    value ?? string.Empty);
            }
        }

        var result = new ColumnAnalysisResult();

        foreach (var header in headers)
        {
            var values = columnValues[header];

            // Veri tipini belirle
            var dataType = DetectDataType(values);

            // DatasetColumn oluştur
            var column = new DatasetColumn
            {
                DatasetId = datasetId,
                ColumnName = header,
                DataType = dataType,

                NullCount = values.Count(
                    v => string.IsNullOrWhiteSpace(v)),

                UniqueCount = values
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Distinct()
                    .Count()
            };

            // Sayısal kolon ise istatistikleri hesapla
            if (dataType == DataType.Numeric)
            {
                var numericValues = GetNumericValues(values);

                if (numericValues.Any())
                {
                    column.MinValue =
                        _statisticsService.GetMin(numericValues);

                    column.MaxValue =
                        _statisticsService.GetMax(numericValues);

                    column.AverageValue =
                        _statisticsService.GetAverage(numericValues);

                    column.MedianValue =
                        _statisticsService.GetMedian(numericValues);

                    column.StandardDeviation =
                        _statisticsService.GetStandardDeviation(
                            numericValues);
                }
            }

            // Kolonu sonuca ekle
            result.Columns.Add(column);

            // Kolonun ham değerlerini de sonuca ekle
            result.ColumnValues.Add(
                header,
                values);
        }

        return result;
    }

    private List<double> GetNumericValues(
        IEnumerable<string> values)
    {
        return values
            .Where(v =>
                double.TryParse(
                    v,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out _))
            .Select(v =>
                double.Parse(
                    v,
                    CultureInfo.InvariantCulture))
            .ToList();
    }

    private DataType DetectDataType(
        IEnumerable<string> values)
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
        {
            return DataType.Numeric;
        }

        if (nonEmptyValues.All(v =>
            bool.TryParse(v, out _)))
        {
            return DataType.Boolean;
        }

        if (nonEmptyValues.All(v =>
            DateTime.TryParse(v, out _)))
        {
            return DataType.DateTime;
        }

        return DataType.String;
    }
}