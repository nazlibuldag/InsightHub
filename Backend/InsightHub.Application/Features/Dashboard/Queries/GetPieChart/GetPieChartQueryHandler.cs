using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetPieChart;

public class GetPieChartQueryHandler
    : IRequestHandler<GetPieChartQuery, List<GetPieChartResponse>>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetColumnValueRepository _datasetColumnValueRepository;

    public GetPieChartQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetColumnValueRepository datasetColumnValueRepository)
    {
        _datasetRepository = datasetRepository;
        _datasetColumnValueRepository = datasetColumnValueRepository;
    }

    public async Task<List<GetPieChartResponse>> Handle(
        GetPieChartQuery request,
        CancellationToken cancellationToken)
    {
        // Dataset var mı kontrol et
        var dataset = await _datasetRepository.GetByIdAsync(
            request.DatasetId,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        // İstenen kolonu bul
        var column = dataset.Columns
            .FirstOrDefault(x =>
                x.ColumnName == request.ColumnName);

        if (column == null)
            throw new Exception("Kolon bulunamadı.");

        // Kolona ait kategorik değerleri getir
        var values = await _datasetColumnValueRepository.GetByColumnIdAsync(
            column.Id,
            cancellationToken);

        return values
            .Select(x => new GetPieChartResponse
            {
                Label = x.Value,
                Count = x.Count
            })
            .OrderByDescending(x => x.Count)
            .ToList();
    }
}