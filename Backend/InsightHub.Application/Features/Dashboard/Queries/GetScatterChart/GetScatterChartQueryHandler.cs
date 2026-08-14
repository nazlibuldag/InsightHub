using InsightHub.Application.Interfaces;
using MediatR;
using System.Globalization;
using System.Text.Json;

namespace InsightHub.Application.Features.Dashboard.Queries.GetScatterChart;

public class GetScatterChartQueryHandler
    : IRequestHandler<GetScatterChartQuery, List<GetScatterChartResponse>>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetScatterChartQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<List<GetScatterChartResponse>> Handle(
        GetScatterChartQuery request,
        CancellationToken cancellationToken)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var result = new List<GetScatterChartResponse>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.Data))
                continue;

            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(
                    row.Data);

                if (data == null)
                    continue;

                if (!data.TryGetValue(request.XColumnName, out var xValue))
                    continue;

                if (!data.TryGetValue(request.YColumnName, out var yValue))
                    continue;

                if (!double.TryParse(
                    xValue.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var x))
                    continue;

                if (!double.TryParse(
                    yValue.Replace(',', '.'),
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out var y))
                    continue;

                result.Add(new GetScatterChartResponse
                {
                    RowNumber = row.RowNumber,
                    X = x,
                    Y = y
                });
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