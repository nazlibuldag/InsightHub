using InsightHub.Application.Features.Analysis.Queries.GetCorrelation;
using InsightHub.Application.Features.Analysis.Queries.GetCorrelationMatrix;
using InsightHub.Application.Features.Analysis.Queries.GetDescriptiveStatistics;
using InsightHub.Application.Features.Analysis.Queries.GetDistribution;
using InsightHub.Application.Features.Analysis.Queries.GetOutliers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalysisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{datasetId}/correlation")]
    public async Task<IActionResult> GetCorrelation(
        Guid datasetId,
        [FromQuery] string column1,
        [FromQuery] string column2)
    {
        var result = await _mediator.Send(
            new GetCorrelationQuery
            {
                DatasetId = datasetId,
                Column1 = column1,
                Column2 = column2
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/correlation-matrix")]
    public async Task<IActionResult> GetCorrelationMatrix(Guid datasetId)
    {
        var result = await _mediator.Send(
            new GetCorrelationMatrixQuery
            {
                DatasetId = datasetId
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/outliers")]
    public async Task<IActionResult> GetOutliers(
    Guid datasetId,
    [FromQuery] string columnName)
    {
        var result = await _mediator.Send(
            new GetOutliersQuery
            {
                DatasetId = datasetId,
                ColumnName = columnName
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/distribution")]
    public async Task<IActionResult> GetDistribution(
    Guid datasetId,
    [FromQuery] string columnName,
    [FromQuery] int binCount = 10)
    {
        var result = await _mediator.Send(
            new GetDistributionQuery
            {
                DatasetId = datasetId,
                ColumnName = columnName,
                BinCount = binCount
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/statistics")]
    public async Task<IActionResult> GetDescriptiveStatistics(
    Guid datasetId,
    [FromQuery] string columnName)
    {
        var result = await _mediator.Send(
            new GetDescriptiveStatisticsQuery
            {
                DatasetId = datasetId,
                ColumnName = columnName
            });

        if (result is null)
            return NotFound("Sayısal kolon bulunamadı veya veri bulunamadı.");

        return Ok(result);
    }
}