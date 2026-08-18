using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Analysis.Queries.GetAiInsights;

public class GetAiInsightsQuery : IRequest<GetAiInsightsResponse>
{
    public Guid DatasetId { get; set; }
}

public class GetAiInsightsResponse
{
    public Guid DatasetId { get; set; }

    public string Insights { get; set; } = string.Empty;
}

public class GetAiInsightsQueryHandler : IRequestHandler<GetAiInsightsQuery, GetAiInsightsResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IAiAnalysisService _aiAnalysisService;
    private readonly ICacheService _cacheService;

    public GetAiInsightsQueryHandler(
        IDatasetRepository datasetRepository,
        IAiAnalysisService aiAnalysisService,
        ICacheService cacheService)
    {
        _datasetRepository = datasetRepository;
        _aiAnalysisService = aiAnalysisService;
        _cacheService = cacheService;
    }

    public async Task<GetAiInsightsResponse> Handle(GetAiInsightsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"ai:insights:{request.DatasetId}";
        var cachedInsights = await _cacheService.GetAsync<GetAiInsightsResponse>(cacheKey, cancellationToken);
        if (cachedInsights != null)
        {
            return cachedInsights;
        }

        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new KeyNotFoundException("Dataset bulunamadı.");
        }

        var columnSummaryList = dataset.Columns.Select(c => new
        {
            c.ColumnName,
            DataType = c.DataType.ToString(),
            c.NullCount,
            c.UniqueCount,
            c.MinValue,
            c.MaxValue,
            c.AverageValue,
            c.MedianValue,
            c.StandardDeviation
        });

        var columnSummary = JsonSerializer.Serialize(columnSummaryList);
        var statsSummary = $"TotalRows: {dataset.TotalRows}, TotalColumns: {dataset.TotalColumns}";

        var insights = await _aiAnalysisService.GenerateDatasetInsightsAsync(
            dataset.Name,
            dataset.TotalRows,
            dataset.TotalColumns,
            columnSummary,
            statsSummary,
            cancellationToken);

        var response = new GetAiInsightsResponse
        {
            DatasetId = dataset.Id,
            Insights = insights
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromHours(1), cancellationToken);

        return response;
    }
}
