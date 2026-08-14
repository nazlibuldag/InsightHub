using System.Text.Json;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetColumnSummary;

public class GetColumnSummaryQueryHandler
    : IRequestHandler<GetColumnSummaryQuery, GetColumnSummaryResponse?>
{
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IDatasetColumnRepository _datasetColumnRepository;

    public GetColumnSummaryQueryHandler(
        IDatasetRowRepository datasetRowRepository,
        IDatasetColumnRepository datasetColumnRepository)
    {
        _datasetRowRepository = datasetRowRepository;
        _datasetColumnRepository = datasetColumnRepository;
    }

    public async Task<GetColumnSummaryResponse?> Handle(
        GetColumnSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var columns = await _datasetColumnRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var column = columns.FirstOrDefault(
            x => x.ColumnName == request.ColumnName);

        if (column == null)
            return null;

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var values = new List<double>();

        var missingCount = 0;

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            if (!document.RootElement.TryGetProperty(
                    request.ColumnName,
                    out var property))
            {
                missingCount++;
                continue;
            }

            if (property.ValueKind == JsonValueKind.Null)
            {
                missingCount++;
                continue;
            }

            if (property.ValueKind == JsonValueKind.String &&
                string.IsNullOrWhiteSpace(property.GetString()))
            {
                missingCount++;
                continue;
            }

            if (column.DataType == DataType.Numeric)
            {
                if (property.ValueKind == JsonValueKind.Number &&
                    property.TryGetDouble(out var numericValue))
                {
                    values.Add(numericValue);
                }
                else if (property.ValueKind == JsonValueKind.String &&
                         double.TryParse(
                             property.GetString(),
                             out var parsedValue))
                {
                    values.Add(parsedValue);
                }
            }
        }

        double? min = null;
        double? max = null;
        double? mean = null;
        double? median = null;
        double? standardDeviation = null;

        if (values.Count > 0)
        {
            values.Sort();

            min = values.First();
            max = values.Last();
            mean = values.Average();

            median = CalculateMedian(values);

            var variance = values
                .Select(x => Math.Pow(x - mean.Value, 2))
                .Average();

            standardDeviation = Math.Sqrt(variance);
        }

        return new GetColumnSummaryResponse
        {
            ColumnName = column.ColumnName,
            DataType = column.DataType,
            Count = rows.Count,
            MissingCount = missingCount,
            UniqueCount = column.UniqueCount,
            Min = min,
            Max = max,
            Mean = mean,
            Median = median,
            StandardDeviation = standardDeviation
        };
    }

    private static double CalculateMedian(List<double> values)
    {
        var middle = values.Count / 2;

        if (values.Count % 2 == 0)
        {
            return (values[middle - 1] + values[middle]) / 2.0;
        }

        return values[middle];
    }
}