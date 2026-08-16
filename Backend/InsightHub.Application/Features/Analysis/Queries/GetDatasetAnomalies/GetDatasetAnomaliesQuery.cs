using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetDatasetAnomalies;

public record GetDatasetAnomaliesQuery(
    Guid DatasetId,
    double ZThreshold = 2.5
) : IRequest<DatasetAnomalyReportDto>;

public class GetDatasetAnomaliesQueryHandler : IRequestHandler<GetDatasetAnomaliesQuery, DatasetAnomalyReportDto>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IAnomalyDetectionService _anomalyDetectionService;
    private readonly ICacheService _cacheService;

    public GetDatasetAnomaliesQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetRowRepository datasetRowRepository,
        IAnomalyDetectionService anomalyDetectionService,
        ICacheService cacheService)
    {
        _datasetRepository = datasetRepository;
        _datasetRowRepository = datasetRowRepository;
        _anomalyDetectionService = anomalyDetectionService;
        _cacheService = cacheService;
    }

    public async Task<DatasetAnomalyReportDto> Handle(GetDatasetAnomaliesQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"anomalies:{request.DatasetId}:{request.ZThreshold}";
        var cached = await _cacheService.GetAsync<DatasetAnomalyReportDto>(cacheKey, cancellationToken);
        if (cached != null)
        {
            return cached;
        }

        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new Exception("Veri seti bulunamadı.");
        }

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(request.DatasetId, cancellationToken);
        var columnReports = new List<ColumnAnomalyReportDto>();

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
                                if (double.TryParse(rawVal, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedNum))
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

            if (values.Count >= 3)
            {
                var report = _anomalyDetectionService.DetectColumnAnomalies(values, col.ColumnName, request.ZThreshold);
                columnReports.Add(report);
            }
        }

        var result = new DatasetAnomalyReportDto(
            request.DatasetId.ToString(),
            columnReports
        );

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30), cancellationToken);

        return result;
    }
}
