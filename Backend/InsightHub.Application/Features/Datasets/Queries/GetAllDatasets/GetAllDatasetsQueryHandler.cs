using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetAllDatasets;

public class GetAllDatasetsQueryHandler: IRequestHandler<GetAllDatasetsQuery, List<GetAllDatasetsResponse>>
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDatasetsQueryHandler(
        IDatasetRepository datasetRepository,
        ICurrentUserService currentUserService)
    {
        _datasetRepository = datasetRepository;
        _currentUserService = currentUserService;
    }

    public async Task<List<GetAllDatasetsResponse>> Handle(
        GetAllDatasetsQuery request,
        CancellationToken cancellationToken)
    {
        var datasets = _currentUserService.IsAdmin
            ? await _datasetRepository.GetAllAsync(cancellationToken)
            : await _datasetRepository.GetAllByUserIdAsync(_currentUserService.UserId, cancellationToken);

        return datasets
            .Select(x => new GetAllDatasetsResponse
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                TotalRows = x.TotalRows,
                TotalColumns = x.TotalColumns,
                UploadedAt = x.UploadedAt
            })
            .ToList();
    }
}