using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetById;

public class GetDatasetByIdQueryHandler
    : IRequestHandler<GetDatasetByIdQuery, GetDatasetByIdResponse?>
{
    private readonly IDatasetRepository _datasetRepository;

    public GetDatasetByIdQueryHandler(IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<GetDatasetByIdResponse?> Handle(
        GetDatasetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (dataset is null)
            return null;

        return new GetDatasetByIdResponse
        {
            Id = dataset.Id,
            Name = dataset.Name,
            Description = dataset.Description,
            TotalRows = dataset.TotalRows,
            TotalColumns = dataset.TotalColumns,
            UploadedAt = dataset.UploadedAt,

            Columns = dataset.Columns
                .Select(column => new DatasetColumnResponse
                {
                    ColumnName = column.ColumnName,
                    DataType = column.DataType,
                    NullCount = column.NullCount,
                    UniqueCount = column.UniqueCount,
                    MinValue = column.MinValue,
                    MaxValue = column.MaxValue,
                    AverageValue = column.AverageValue,
                    MedianValue = column.MedianValue,
                    StandardDeviation = column.StandardDeviation
                })
                .ToList()
        };
    }
}