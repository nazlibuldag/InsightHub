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

    public GetDatasetSummaryQueryHandler(IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<GetDatasetSummaryResponse> Handle(
        GetDatasetSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(
            request.Id,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        return new GetDatasetSummaryResponse
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
    }
}