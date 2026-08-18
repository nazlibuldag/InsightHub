using System.Globalization;
using System.Text.Json;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDescriptiveStatistics;

public class GetDescriptiveStatisticsQueryHandler
    : IRequestHandler<GetDescriptiveStatisticsQuery, GetDescriptiveStatisticsResponse?>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetDescriptiveStatisticsQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetDescriptiveStatisticsResponse?> Handle(
        GetDescriptiveStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        if (!rows.Any())
        {
            throw new KeyNotFoundException("Belirtilen dataset bulunamadı veya verisi yok.");
        }

        var values = new List<double>();

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            if (!document.RootElement.TryGetProperty(
                    request.ColumnName,
                    out var property))
            {
                continue;
            }

            double numericValue;

            // JSON içerisinde gerçek number ise
            if (property.ValueKind == JsonValueKind.Number &&
                property.TryGetDouble(out numericValue))
            {
                values.Add(numericValue);
            }
            // JSON içerisinde string ise
            else if (property.ValueKind == JsonValueKind.String &&
                     double.TryParse(
                         property.GetString(),
                         NumberStyles.Float,
                         CultureInfo.InvariantCulture,
                         out numericValue))
            {
                values.Add(numericValue);
            }
        }

        if (values.Count == 0)
        {
            throw new ArgumentException($"'{request.ColumnName}' kolonu sayısal veri içermiyor veya kolon bulunamadı.");
        }

        values.Sort();

        var count = values.Count;

        // Ortalama
        var mean = values.Average();

        // Medyan
        var median = CalculateMedian(values);

        // Mod
        var mode = CalculateMode(values);

        // Minimum
        var min = values.First();

        // Maximum
        var max = values.Last();

        // Range
        var range = max - min;

        // Q1
        var lowerHalf = values
            .Take(count / 2)
            .ToList();

        var q1 = CalculateMedian(lowerHalf);

        // Q3
        var upperHalf = values
            .Skip((count + 1) / 2)
            .ToList();

        var q3 = CalculateMedian(upperHalf);

        // IQR
        var iqr = q3 - q1;

        // Varyans
        var variance = values
            .Select(x => Math.Pow(x - mean, 2))
            .Average();

        // Standart sapma
        var standardDeviation = Math.Sqrt(variance);

        return new GetDescriptiveStatisticsResponse
        {
            ColumnName = request.ColumnName,
            Count = count,
            Mean = mean,
            Median = median,
            Mode = mode,
            Min = min,
            Max = max,
            Range = range,
            Q1 = q1,
            Q3 = q3,
            IQR = iqr,
            Variance = variance,
            StandardDeviation = standardDeviation
        };
    }

    private static double CalculateMedian(List<double> values)
    {
        if (values.Count == 0)
            return 0;

        var middle = values.Count / 2;

        if (values.Count % 2 == 0)
        {
            return (
                values[middle - 1] +
                values[middle]
            ) / 2.0;
        }

        return values[middle];
    }

    private static double? CalculateMode(List<double> values)
    {
        if (values.Count == 0)
            return null;

        var mode = values
            .GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .First();

        return mode.Key;
    }
}