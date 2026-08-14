using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

public class GetDatasetDataQueryHandler
    : IRequestHandler<GetDatasetDataQuery, GetDatasetDataResponse>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRowRepository _datasetRowRepository;

    public GetDatasetDataQueryHandler(
        IDatasetRepository datasetRepository,
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRepository = datasetRepository;
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<GetDatasetDataResponse> Handle(
        GetDatasetDataQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.DatasetId,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 20;

        var allRows = await _datasetRowRepository.GetByDatasetIdAsync(
            request.DatasetId,
            cancellationToken);

        var totalRows = allRows.Count;

        var totalPages = (int)Math.Ceiling(
            totalRows / (double)request.PageSize);

        var rows = allRows
            .OrderBy(x => x.RowNumber)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new DatasetRowResponse
            {
                RowNumber = x.RowNumber,
                Data = x.Data
            })
            .ToList();

        return new GetDatasetDataResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRows = totalRows,
            TotalPages = totalPages,
            Rows = rows
        };
    }
}