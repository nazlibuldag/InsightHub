using System.Text.Json;
using InsightHub.Application.Interfaces;
using MediatR;
using System.Globalization;

namespace InsightHub.Application.Features.Analysis.Queries.GetOutliers;

public class GetOutliersQueryHandler
    : IRequestHandler<GetOutliersQuery, GetOutliersResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetOutliersQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetOutliersResponse> Handle(
        GetOutliersQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var values = new List<(int RowNumber, double Value)>();

        foreach (var row in rows)
        {
            using var document = JsonDocument.Parse(row.Data);

            if (!document.RootElement.TryGetProperty(
                    request.ColumnName,
                    out var property))
            {
                continue;
            }

            double value;
            if (property.ValueKind == JsonValueKind.Number && property.TryGetDouble(out value))
            {
                values.Add((row.RowNumber, value));
            }
            else if (property.ValueKind == JsonValueKind.String && double.TryParse(property.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                values.Add((row.RowNumber, value));
            }
        }

        if (values.Count < 4)
        {
            throw new Exception(
                "Aykırı değer analizi için yeterli veri bulunamadı.");
        }

        var sortedValues = values
            .Select(x => x.Value)
            .OrderBy(x => x)
            .ToList();

        var q1 = CalculatePercentile(sortedValues, 0.25);
        var q3 = CalculatePercentile(sortedValues, 0.75);

        var iqr = q3 - q1;

        var lowerBound = q1 - (1.5 * iqr);
        var upperBound = q3 + (1.5 * iqr);

        var outliers = values
            .Where(x =>
                x.Value < lowerBound ||
                x.Value > upperBound)
            .OrderBy(x => x.Value)
            .Select(x => new OutlierValueResponse
            {
                RowNumber = x.RowNumber,
                Value = x.Value
            })
            .ToList();

        return new GetOutliersResponse
        {
            ColumnName = request.ColumnName,
            Q1 = q1,
            Q3 = q3,
            IQR = iqr,
            LowerBound = lowerBound,
            UpperBound = upperBound,
            OutlierCount = outliers.Count,
            Outliers = outliers
        };
    }

    private static double CalculatePercentile(
        List<double> sortedValues,
        double percentile)
    {
        var position =
            (sortedValues.Count - 1) * percentile;

        var lowerIndex = (int)Math.Floor(position);
        var upperIndex = (int)Math.Ceiling(position);

        if (lowerIndex == upperIndex)
        {
            return sortedValues[lowerIndex];
        }

        var weight = position - lowerIndex;

        return sortedValues[lowerIndex]
               + weight *
               (sortedValues[upperIndex] -
                sortedValues[lowerIndex]);
    }
}