using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.SortDataset;

public class SortDatasetQueryHandler
    : IRequestHandler<SortDatasetQuery, List<SortDatasetResponse>>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public SortDatasetQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<List<SortDatasetResponse>> Handle(
        SortDatasetQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var result = new List<(SortDatasetResponse Row, double? NumericValue, string? StringValue)>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Data))
                continue;

            try
            {
                var data = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, string>>(row.Data);

                if (data == null)
                    continue;

                if (!data.TryGetValue(request.ColumnName, out var value))
                    continue;

                var normalizedValue = value.Replace(",", ".");

                if (double.TryParse(
                    normalizedValue,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var numericValue))
                {
                    result.Add((
                        new SortDatasetResponse
                        {
                            RowNumber = row.RowNumber,
                            Data = row.Data
                        },
                        numericValue,
                        null));
                }
                else
                {
                    result.Add((
                        new SortDatasetResponse
                        {
                            RowNumber = row.RowNumber,
                            Data = row.Data
                        },
                        null,
                        value));
                }
            }
            catch
            {
                continue;
            }
        }

        if (request.SortOrder.Equals(
            "desc",
            StringComparison.OrdinalIgnoreCase))
        {
            return result
                .OrderByDescending(x => x.NumericValue.HasValue
                    ? x.NumericValue.Value
                    : double.MinValue)
                .ThenByDescending(x => x.StringValue)
                .Select(x => x.Row)
                .ToList();
        }

        return result
            .OrderBy(x => x.NumericValue.HasValue
                ? x.NumericValue.Value
                : double.MaxValue)
            .ThenBy(x => x.StringValue)
            .Select(x => x.Row)
            .ToList();
    }
}