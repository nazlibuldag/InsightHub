using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetSummary;

public class GetDatasetSummaryQueryHandler
    : IRequestHandler<GetDatasetSummaryQuery, GetDatasetSummaryResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly ICacheService _cacheService;

    public GetDatasetSummaryQueryHandler(
        IDatasetRepository datasetRepository,
        ICacheService cacheService)
    {
        _datasetRepository = datasetRepository;
        _cacheService = cacheService;
    }

    public async Task<GetDatasetSummaryResponse> Handle(
        GetDatasetSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"dataset:summary:{request.Id}";
        var cachedSummary = await _cacheService.GetAsync<GetDatasetSummaryResponse>(cacheKey, cancellationToken);
        if (cachedSummary != null)
        {
            return cachedSummary;
        }

        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        var response = new GetDatasetSummaryResponse
        {
            Id = dataset.Id,
            Name = dataset.Name,
            TotalRows = dataset.TotalRows,
            TotalColumns = dataset.TotalColumns,

            NumericColumns = dataset.Columns.Count(c => c.DataType == DataType.Numeric),
            StringColumns = dataset.Columns.Count(c => c.DataType == DataType.String),
            BooleanColumns = dataset.Columns.Count(c => c.DataType == DataType.Boolean),
            DateColumns = dataset.Columns.Count(c => c.DataType == DataType.DateTime),

            TotalMissingValues = dataset.Columns.Sum(c => c.NullCount)
        };

        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(15), cancellationToken);

        return response;
    }
}