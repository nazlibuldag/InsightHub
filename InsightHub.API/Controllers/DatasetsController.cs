using InsightHub.Application.Features.Datasets.Commands.CreateDataset;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using InsightHub.Application.Features.Datasets.Commands.UploadDataset;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DatasetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DatasetsController(IMediator mediator)
    {
        _mediator = mediator;
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
}