using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.SearchDataset;

public class SearchDatasetQueryHandler
    : IRequestHandler<SearchDatasetQuery, SearchDatasetResponse>
{
    private readonly IDatasetRowRepository _datasetRowRepository;

    public SearchDatasetQueryHandler(
        IDatasetRowRepository datasetRowRepository)
    {
        _datasetRowRepository = datasetRowRepository;
    }

    public async Task<SearchDatasetResponse> Handle(
        SearchDatasetQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 20;

        var rows = await _datasetRowRepository.SearchAsync(
            request.DatasetId,
            request.SearchTerm,
            cancellationToken);

        var totalRows = rows.Count;

        var totalPages = (int)Math.Ceiling(
            totalRows / (double)request.PageSize);

        var pagedRows = rows
            .OrderBy(x => x.RowNumber)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new SearchDatasetRowResponse
            {
                RowNumber = x.RowNumber,
                Data = x.Data
            })
            .ToList();

        return new SearchDatasetResponse
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalRows = totalRows,
            TotalPages = totalPages,
            Rows = pagedRows
        };
    }
}