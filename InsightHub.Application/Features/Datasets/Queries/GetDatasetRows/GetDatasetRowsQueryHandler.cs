using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;

public class GetDatasetRowsQueryHandler
    : IRequestHandler<GetDatasetRowsQuery, GetDatasetRowsResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetDatasetRowsQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetDatasetRowsResponse> Handle(
        GetDatasetRowsQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var filteredRows = new List<(int RowNumber, string Data)>();

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

                // SEARCH
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchMatch = data.Values.Any(x =>
                        x.Contains(
                            request.SearchTerm,
                            StringComparison.OrdinalIgnoreCase));

                    if (!searchMatch)
                        continue;
                }

                // FILTER
                if (!string.IsNullOrWhiteSpace(request.FilterColumn) &&
                    !string.IsNullOrWhiteSpace(request.FilterOperator) &&
                    !string.IsNullOrWhiteSpace(request.FilterValue))
                {
                    if (!data.TryGetValue(
                        request.FilterColumn,
                        out var columnValue))
                    {
                        continue;
                    }

                    var normalizedColumnValue = columnValue.Replace(",", ".");
                    var normalizedFilterValue = request.FilterValue.Replace(",", ".");

                    bool matches;

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
                        matches = request.FilterOperator switch
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
                    else
                    {
                        matches = request.FilterOperator switch
                        {
                            "=" => columnValue.Equals(
                                request.FilterValue,
                                StringComparison.OrdinalIgnoreCase),

                            "==" => columnValue.Equals(
                                request.FilterValue,
                                StringComparison.OrdinalIgnoreCase),

                            "!=" => !columnValue.Equals(
                                request.FilterValue,
                                StringComparison.OrdinalIgnoreCase),

                            "contains" => columnValue.Contains(
                                request.FilterValue,
                                StringComparison.OrdinalIgnoreCase),

                            _ => false
                        };
                    }

                    if (!matches)
                        continue;
                }

                filteredRows.Add((row.RowNumber, row.Data));
            }
            catch
            {
                continue;
            }
        }

        // SORT
        if (!string.IsNullOrWhiteSpace(request.SortColumn))
        {
            var sortedRows = new List<(int RowNumber, string Data, double? NumericValue, string? StringValue)>();

            foreach (var row in filteredRows)
            {
                try
                {
                    var data = System.Text.Json.JsonSerializer
                        .Deserialize<Dictionary<string, string>>(row.Data);

                    if (data == null ||
                        !data.TryGetValue(request.SortColumn, out var value))
                    {
                        continue;
                    }

                    var normalizedValue = value.Replace(",", ".");

                    if (double.TryParse(
                        normalizedValue,
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var numericValue))
                    {
                        sortedRows.Add((
                            row.RowNumber,
                            row.Data,
                            numericValue,
                            null));
                    }
                    else
                    {
                        sortedRows.Add((
                            row.RowNumber,
                            row.Data,
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
                filteredRows = sortedRows
                    .OrderByDescending(x => x.NumericValue.HasValue
                        ? x.NumericValue.Value
                        : double.MinValue)
                    .ThenByDescending(x => x.StringValue)
                    .Select(x => (x.RowNumber, x.Data))
                    .ToList();
            }
            else
            {
                filteredRows = sortedRows
                    .OrderBy(x => x.NumericValue.HasValue
                        ? x.NumericValue.Value
                        : double.MaxValue)
                    .ThenBy(x => x.StringValue)
                    .Select(x => (x.RowNumber, x.Data))
                    .ToList();
            }
        }
        else
        {
            filteredRows = filteredRows
                .OrderBy(x => x.RowNumber)
                .ToList();
        }

        // PAGINATION
        var totalRows = filteredRows.Count;

        var page = request.Page < 1
            ? 1
            : request.Page;

        var pageSize = request.PageSize < 1
            ? 20
            : request.PageSize;

        var totalPages = (int)Math.Ceiling(
            totalRows / (double)pageSize);

        var pagedRows = filteredRows
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new GetDatasetRowsItem
            {
                RowNumber = x.RowNumber,
                Data = x.Data
            })
            .ToList();

        return new GetDatasetRowsResponse
        {
            Page = page,
            PageSize = pageSize,
            TotalRows = totalRows,
            TotalPages = totalPages,
            Rows = pagedRows
        };
    }
}