using InsightHub.Application.Features.Dashboard.Queries.GetBarChart;
using InsightHub.Application.Features.Dashboard.Queries.GetDashboardSummary;
using InsightHub.Application.Features.Datasets.Queries.GetPieChart;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{datasetId}")]
    public async Task<IActionResult> GetDashboardSummary(Guid datasetId)
    {
        var result = await _mediator.Send(
            new GetDashboardSummaryQuery
            {
                DatasetId = datasetId
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/charts/bar")]
    public async Task<IActionResult> GetBarChart(Guid datasetId)
    {
        var result = await _mediator.Send(
            new GetBarChartQuery
            {
                DatasetId = datasetId
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/charts/pie")]
    public async Task<IActionResult> GetPieChart(
        Guid datasetId,
        [FromQuery] string columnName)
    {
        var result = await _mediator.Send(
            new GetPieChartQuery
            {
                DatasetId = datasetId,
                ColumnName = columnName
            });

        return Ok(result);
    }
}