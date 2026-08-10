using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetBarChart;

public class GetBarChartQueryHandler
    : IRequestHandler<GetBarChartQuery, List<GetBarChartResponse>>
{
    private readonly IDatasetRepository _datasetRepository;

    public GetBarChartQueryHandler(
        IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<List<GetBarChartResponse>> Handle(
        GetBarChartQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.DatasetId,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        return dataset.Columns
            .Where(x => x.DataType == DataType.Numeric)
            .Select(x => new GetBarChartResponse
            {
                ColumnName = x.ColumnName,
                Average = x.AverageValue ?? 0
            })
            .OrderByDescending(x => x.Average)
            .ToList();
    }
}