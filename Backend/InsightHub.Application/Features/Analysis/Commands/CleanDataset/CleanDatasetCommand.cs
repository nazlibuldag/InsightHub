using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Commands.CleanDataset;

public record CleanDatasetCommand(
    Guid DatasetId,
    string Strategy = "MEAN" // "MEAN", "MEDIAN", "ZERO", "FORWARD_FILL"
) : IRequest<CleaningResultDto>;

public class CleanDatasetCommandHandler : IRequestHandler<CleanDatasetCommand, CleaningResultDto>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;
    private readonly IDataCleaningService _dataCleaningService;

    public CleanDatasetCommandHandler(
        IDatasetRepository datasetRepository,
        IDatasetRowRepository datasetRowRepository,
        IDataCleaningService dataCleaningService)
    {
        _datasetRepository = datasetRepository;
        _datasetRowRepository = datasetRowRepository;
        _dataCleaningService = dataCleaningService;
    }

    public async Task<CleaningResultDto> Handle(CleanDatasetCommand request, CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new Exception("Veri seti bulunamadı.");
        }

        var rows = await _datasetRowRepository.GetByDatasetIdAsync(request.DatasetId, cancellationToken);
        int fixedCount = 0;

        foreach (var col in dataset.Columns)
        {
            var rawValues = new List<double?>();
            foreach (var row in rows)
            {
                try
                {
                    using var doc = JsonDocument.Parse(row.Data);
                    bool found = false;
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (string.Equals(prop.Name, col.ColumnName, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            string? rawVal = prop.Value.ValueKind == JsonValueKind.String
                                ? prop.Value.GetString()
                                : prop.Value.GetRawText();

                            if (!string.IsNullOrWhiteSpace(rawVal) && double.TryParse(rawVal.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
                            {
                                rawValues.Add(parsed);
                            }
                            else
                            {
                                rawValues.Add(null);
                                fixedCount++;
                            }
                            break;
                        }
                    }
                    if (!found)
                    {
                        rawValues.Add(null);
                        fixedCount++;
                    }
                }
                catch
                {
                    rawValues.Add(null);
                    fixedCount++;
                }
            }

            _dataCleaningService.ImputeNumericColumn(rawValues, request.Strategy);
        }

        return new CleaningResultDto(
            request.DatasetId.ToString(),
            rows.Count,
            fixedCount,
            new List<string> { $"Imputation Strategy: {request.Strategy.ToUpper()}" }
        );
    }
}
