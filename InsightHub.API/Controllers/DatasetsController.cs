using InsightHub.Application.Features.Datasets.Commands.CreateDataset;
using InsightHub.Application.Features.Datasets.Commands.DeleteDataset;
using InsightHub.Application.Features.Datasets.Commands.UpdateDataset;
using InsightHub.Application.Features.Datasets.Commands.UploadDataset;
using InsightHub.Application.Features.Datasets.Queries.GetAllDatasets;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetById;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetData;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetSummary;
using InsightHub.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDatasetRowRepository _datasetRowRepository;

    public DatasetsController(IMediator mediator, IDatasetRowRepository datasetRowRepository)
    {
        _mediator = mediator;
        _datasetRowRepository = datasetRowRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDatasetCommand command)
    {
        var datasetId = await _mediator.Send(command);

        return Ok(new
        {
            Id = datasetId,
            Message = "Dataset başarıyla oluşturuldu."
        });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadDatasetCommand command)
    {
        var datasetId = await _mediator.Send(command);

        return Ok(new
        {
            Id = datasetId,
            Message = "CSV dosyası başarıyla yüklendi."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var datasets = await _mediator.Send(new GetAllDatasetsQuery());

        return Ok(datasets);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dataset = await _mediator.Send(new GetDatasetByIdQuery(id));

        if (dataset is null)
        {
            return NotFound();
        }

        return Ok(dataset);
    }

    [HttpGet("{id}/summary")]
    public async Task<IActionResult> GetSummary(Guid id)
    {
        var result = await _mediator.Send(new GetDatasetSummaryQuery(id));

        return Ok(result);
    }

    [HttpGet("{id}/data")]
    public async Task<IActionResult> GetDatasetData(Guid id)
    {
        var result = await _mediator.Send(
            new GetDatasetDataQuery
            {
                DatasetId = id
            });

        return Ok(result);
    }

    [HttpGet("{id}/rows")]
    public async Task<IActionResult> GetDatasetRows(Guid id)
    {
        var rows = await _datasetRowRepository.GetByDatasetIdAsync(
            id,
            HttpContext.RequestAborted);

        return Ok(rows);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id,UpdateDatasetCommand command)
    {
        command.Id = id;

        await _mediator.Send(command);

        return Ok(new
        {
            Message = "Dataset başarıyla güncellendi."
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteDatasetCommand(id));

        return Ok(new
        {
            Message = "Dataset başarıyla silindi."
        });
    }
}