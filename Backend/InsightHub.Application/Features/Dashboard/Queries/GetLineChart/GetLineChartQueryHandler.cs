using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetLineChart;

public class GetLineChartQueryHandler
    : IRequestHandler<GetLineChartQuery, List<GetLineChartResponse>>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetLineChartQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<List<GetLineChartResponse>> Handle(
        GetLineChartQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var result = new List<GetLineChartResponse>();

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

                if (double.TryParse(
    value.Replace(',', '.'),
    System.Globalization.NumberStyles.Any,
    System.Globalization.CultureInfo.InvariantCulture,
    out var numericValue))
                {
                    result.Add(new GetLineChartResponse
                    {
                        RowNumber = row.RowNumber,
                        Value = numericValue
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