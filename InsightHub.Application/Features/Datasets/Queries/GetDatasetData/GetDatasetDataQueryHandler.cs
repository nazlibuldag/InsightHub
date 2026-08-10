using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataQueryHandler
    : IRequestHandler<GetDatasetDataQuery, List<GetDatasetDataResponse>>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetColumnValueRepository _datasetColumnValueRepository;

    public GetDatasetDataQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetColumnValueRepository datasetColumnValueRepository)
    {
        _datasetRepository = datasetRepository;
        _datasetColumnValueRepository = datasetColumnValueRepository;
    }

    public async Task<List<GetDatasetDataResponse>> Handle(
        GetDatasetDataQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.DatasetId,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        var result = new List<GetDatasetDataResponse>();

        foreach (var column in dataset.Columns)
        {
            var values = await _datasetColumnValueRepository.GetByColumnIdAsync(
                column.Id,
                cancellationToken);

            result.Add(new GetDatasetDataResponse
            {
                ColumnName = column.ColumnName,
                Values = values
                    .Select(x => x.Value)
                    .ToList()
            });
        }

        return result;
    }
}