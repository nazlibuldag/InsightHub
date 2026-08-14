using InsightHub.Application.Features.Datasets.Commands.AddDatasetRow;
using InsightHub.Application.Features.Datasets.Commands.CreateDataset;
using InsightHub.Application.Features.Datasets.Commands.DeleteDataset;
using InsightHub.Application.Features.Datasets.Commands.DeleteDatasetRow;
using InsightHub.Application.Features.Datasets.Commands.UpdateDataset;
using InsightHub.Application.Features.Datasets.Commands.UpdateDatasetRow;
using InsightHub.Application.Features.Datasets.Commands.UploadDataset;
using InsightHub.Application.Features.Datasets.Queries.FilterDataset;
using InsightHub.Application.Features.Datasets.Queries.GetAllDatasets;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetById;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetData;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetSummary;
using InsightHub.Application.Features.Datasets.Queries.SearchDataset;
using InsightHub.Application.Features.Datasets.Queries.SortDataset;
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
    public async Task<IActionResult> GetDatasetData(
     Guid id,
     [FromQuery] int page = 1,
     [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new GetDatasetDataQuery
            {
                DatasetId = id,
                Page = page,
                PageSize = pageSize
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
    public async Task<IActionResult> UpdateDataset(
        Guid id,
        [FromBody] UpdateDatasetRequest request)
    {
        var result = await _mediator.Send(
            new UpdateDatasetCommand
            {
                Id = id,
                Name = request.Name,
                Description = request.Description
            });

        if (!result)
            return NotFound("Dataset bulunamadı.");

        return Ok(new
        {
            message = "Dataset başarıyla güncellendi."
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDataset(Guid id)
    {
        var result = await _mediator.Send(
            new DeleteDatasetCommand
            {
                Id = id
            });

        if (!result)
            return NotFound("Dataset bulunamadı.");

        return Ok(new
        {
            message = "Dataset başarıyla silindi."
        });
    }
    [HttpGet("{id}/rows/{rowNumber}")]
    public async Task<IActionResult> GetDatasetRow(
    Guid id,
    int rowNumber)
    {
        var row = await _datasetRowRepository
            .GetByDatasetIdAndRowNumberAsync(
                id,
                rowNumber,
                HttpContext.RequestAborted);

        if (row == null)
            return NotFound("Satır bulunamadı.");

        return Ok(row);
    }

    [HttpGet("{id}/search")]
    public async Task<IActionResult> SearchDataset(
       Guid id,
       [FromQuery] string? searchTerm,
       [FromQuery] int page = 1,
       [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new SearchDatasetQuery
            {
                DatasetId = id,
                SearchTerm = searchTerm,
                Page = page,
                PageSize = pageSize
            });

        return Ok(result);
    }

    [HttpGet("{id}/filter")]
    public async Task<IActionResult> FilterDataset(
    Guid id,
    [FromQuery] string columnName,
    [FromQuery] string @operator,
    [FromQuery] string value)
    {
        var result = await _mediator.Send(
            new FilterDatasetQuery
            {
                DatasetId = id,
                ColumnName = columnName,
                Operator = @operator,
                Value = value
            });

        return Ok(result);
    }

    [HttpGet("{id}/sort")]
    public async Task<IActionResult> SortDataset(
    Guid id,
    [FromQuery] string columnName,
    [FromQuery] string sortOrder = "asc")
    {
        var result = await _mediator.Send(
            new SortDatasetQuery
            {
                DatasetId = id,
                ColumnName = columnName,
                SortOrder = sortOrder
            });

        return Ok(result);
    }


    [HttpGet("{id}/query")]
    public async Task<IActionResult> QueryDataset(
    Guid id,
    [FromQuery] string? searchTerm,
    [FromQuery] string? filterColumn,
    [FromQuery] string? filterOperator,
    [FromQuery] string? filterValue,
    [FromQuery] string? sortColumn,
    [FromQuery] string sortOrder = "asc",
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(
            new GetDatasetRowsQuery
            {
                DatasetId = id,
                SearchTerm = searchTerm,
                FilterColumn = filterColumn,
                FilterOperator = filterOperator,
                FilterValue = filterValue,
                SortColumn = sortColumn,
                SortOrder = sortOrder,
                Page = page,
                PageSize = pageSize
            });

        return Ok(result);
    }

    [HttpPost("{id}/rows")]
    public async Task<IActionResult> AddDatasetRow(
    Guid id,
    [FromBody] AddDatasetRowCommand command)
    {
        command.DatasetId = id;

        var result = await _mediator.Send(command);

        return Ok(new
        {
            message = "Dataset satırı başarıyla eklendi."
        });
    }


    [HttpPut("{id}/rows/{rowNumber}")]
    public async Task<IActionResult> UpdateDatasetRow(
    Guid id,
    int rowNumber,
    [FromBody] string data)
    {
        var result = await _mediator.Send(
            new UpdateDatasetRowCommand
            {
                DatasetId = id,
                RowNumber = rowNumber,
                Data = data
            });

        if (!result)
            return NotFound("Satır bulunamadı.");

        return Ok(new
        {
            message = "Dataset satırı başarıyla güncellendi."
        });
    }

    [HttpDelete("{id}/rows/{rowNumber}")]
    public async Task<IActionResult> DeleteDatasetRow(
    Guid id,
    int rowNumber)
    {
        var result = await _mediator.Send(
            new DeleteDatasetRowCommand
            {
                DatasetId = id,
                RowNumber = rowNumber
            });

        if (!result)
            return NotFound("Satır bulunamadı.");

        return Ok(new
        {
            message = "Dataset satırı başarıyla silindi."
        });
    }
}