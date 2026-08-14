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

    public GetAllDatasetsQueryHandler(IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<List<GetAllDatasetsResponse>> Handle(
        GetAllDatasetsQuery request,
        CancellationToken cancellationToken)
    {
        var datasets = await _datasetRepository.GetAllAsync(cancellationToken);

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