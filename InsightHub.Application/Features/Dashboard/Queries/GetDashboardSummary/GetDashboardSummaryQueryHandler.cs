using InsightHub.Application.Interfaces;
using InsightHub.Domain.Enums;
using MediatR;

namespace InsightHub.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, GetDashboardSummaryResponse>
{
    private readonly IDatasetRepository _datasetRepository;

    public GetDashboardSummaryQueryHandler(
        IDatasetRepository datasetRepository)
    {
        _datasetRepository = datasetRepository;
    }

    public async Task<GetDashboardSummaryResponse> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(
            request.DatasetId,
            cancellationToken);

        if (dataset == null)
            throw new Exception("Dataset bulunamadı.");

        return new GetDashboardSummaryResponse
        {
            DatasetName = dataset.Name,

            TotalRows = dataset.TotalRows,

            TotalColumns = dataset.TotalColumns,

            NumericColumns = dataset.Columns.Count(
                x => x.DataType == DataType.Numeric),

            StringColumns = dataset.Columns.Count(
                x => x.DataType == DataType.String),

            DateColumns = dataset.Columns.Count(
                x => x.DataType == DataType.DateTime),

            BooleanColumns = dataset.Columns.Count(
                x => x.DataType == DataType.Boolean),

            TotalMissingValues = dataset.Columns.Sum(
                x => x.NullCount),

            Columns = dataset.Columns
                .Select(x => new DashboardColumnResponse
                {
                    ColumnName = x.ColumnName,
                    DataType = x.DataType
                })
                .ToList()
        };
    }
}