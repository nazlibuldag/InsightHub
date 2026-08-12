using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.FilterDataset;

public class FilterDatasetQueryHandler
    : IRequestHandler<FilterDatasetQuery, List<FilterDatasetResponse>>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public FilterDatasetQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<List<FilterDatasetResponse>> Handle(
        FilterDatasetQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var result = new List<FilterDatasetResponse>();

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

                if (!data.TryGetValue(request.ColumnName, out var columnValue))
                    continue;

                bool matches = false;

                // Virgüllü ondalık değerleri noktaya çevir
                var normalizedColumnValue = columnValue
                    .Replace(",", ".");

                var normalizedFilterValue = request.Value
                    .Replace(",", ".");

                // Sayısal filtreleme
                if (double.TryParse(
                        normalizedColumnValue,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var numericValue)
                    &&
                    double.TryParse(
                        normalizedFilterValue,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var filterValue))
                {
                    matches = request.Operator switch
                    {
                        ">" => numericValue > filterValue,
                        "<" => numericValue < filterValue,
                        ">=" => numericValue >= filterValue,
                        "<=" => numericValue <= filterValue,
                        "=" => numericValue == filterValue,
                        "==" => numericValue == filterValue,
                        "!=" => numericValue != filterValue,
                        _ => false
                    };
                }
                // String filtreleme
                else
                {
                    matches = request.Operator switch
                    {
                        "=" => columnValue.Equals(
                            request.Value,
                            StringComparison.OrdinalIgnoreCase),

                        "==" => columnValue.Equals(
                            request.Value,
                            StringComparison.OrdinalIgnoreCase),

                        "!=" => !columnValue.Equals(
                            request.Value,
                            StringComparison.OrdinalIgnoreCase),

                        "contains" => columnValue.Contains(
                            request.Value,
                            StringComparison.OrdinalIgnoreCase),

                        _ => false
                    };
                }

                if (matches)
                {
                    result.Add(new FilterDatasetResponse
                    {
                        RowNumber = row.RowNumber,
                        Data = row.Data
                    });
                }
            }
            catch
            {
                continue;
            }
        }

        return result
            .OrderBy(x => x.RowNumber)
            .ToList();
    }
}