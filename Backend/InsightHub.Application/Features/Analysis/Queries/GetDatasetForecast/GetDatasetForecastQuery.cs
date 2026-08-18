using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDatasetForecast;

public class GetDatasetForecastQuery : IRequest<GetDatasetForecastResponse>
{
    public Guid DatasetId { get; set; }

    public int StepsAhead { get; set; } = 5;
}

public class GetDatasetForecastResponse
{
    public Guid DatasetId { get; set; }

    public List<ForecastingResultDto> ColumnForecasts { get; set; } = new();
}

public class GetDatasetForecastQueryHandler : IRequestHandler<GetDatasetForecastQuery, GetDatasetForecastResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IMlForecastingService _mlForecastingService;
    private readonly ICacheService _cacheService;

    public GetDatasetForecastQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetRowRepository datasetRowRepository,
        IMlForecastingService mlForecastingService,
        ICacheService cacheService)
    {
        _datasetRepository = datasetRepository;
        _datasetRowRepository = datasetRowRepository;
        _mlForecastingService = mlForecastingService;
        _cacheService = cacheService;
    }

    public async Task<GetDatasetForecastResponse> Handle(GetDatasetForecastQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"forecast:{request.DatasetId}:{request.StepsAhead}";
        var cachedForecast = await _cacheService.GetAsync<GetDatasetForecastResponse>(cacheKey, cancellationToken);
        if (cachedForecast != null)
        {
            return cachedForecast;
        }

        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new KeyNotFoundException("Dataset bulunamadı.");
        }

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(request.DatasetId, cancellationToken);
        var columnForecasts = new List<ForecastingResultDto>();
        foreach (var col in dataset.Columns)
        {
            var values = new List<double>();
            foreach (var row in rows)
            {
                try
                {
                    using var doc = JsonDocument.Parse(row.Data);
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (string.Equals(prop.Name, col.ColumnName, StringComparison.OrdinalIgnoreCase))
                        {
                            string? rawVal = prop.Value.ValueKind == JsonValueKind.String
                                ? prop.Value.GetString()
                                : prop.Value.GetRawText();

                            if (!string.IsNullOrWhiteSpace(rawVal))
                            {
                                rawVal = rawVal.Trim().Replace(',', '.');
                                if (double.TryParse(rawVal, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var parsedNum))
                                {
                                    values.Add(parsedNum);
                                }
                            }
                            break;
                        }
                    }
                }
                catch { }
            }

            if (values.Count >= 2)
            {
                var forecastResult = _mlForecastingService.ForecastColumnTrend(values, col.ColumnName, request.StepsAhead);
                columnForecasts.Add(forecastResult);
            }
        }

        var response = new GetDatasetForecastResponse
        {
            DatasetId = dataset.Id,
            ColumnForecasts = columnForecasts
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(30), cancellationToken);

        return response;
    }
}
