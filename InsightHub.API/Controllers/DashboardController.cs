using InsightHub.Application.Features.Dashboard.Queries.GetBarChart;
using InsightHub.Application.Features.Dashboard.Queries.GetDashboardSummary;
using InsightHub.Application.Features.Dashboard.Queries.GetLineChart;
using InsightHub.Application.Features.Dashboard.Queries.GetPieChart;
using InsightHub.Application.Features.Dashboard.Queries.GetScatterChart;
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

    [HttpGet("{datasetId}/charts/line")]
    public async Task<IActionResult> GetLineChart(
    Guid datasetId,
    [FromQuery] string columnName)
    {
        var result = await _mediator.Send(
            new GetLineChartQuery
            {
                DatasetId = datasetId,
                ColumnName = columnName
            });

        return Ok(result);
    }

    [HttpGet("{datasetId}/charts/scatter")]
    public async Task<IActionResult> GetScatterChart(
    Guid datasetId,
    [FromQuery] string xColumnName,
    [FromQuery] string yColumnName)
    {
        var result = await _mediator.Send(
            new GetScatterChartQuery
            {
                DatasetId = datasetId,
                XColumnName = xColumnName,
                YColumnName = yColumnName
            });

        return Ok(result);
    }
}