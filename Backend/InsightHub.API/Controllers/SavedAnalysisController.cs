using System;
using System.Security.Claims;
using System.Threading.Tasks;
using InsightHub.Application.Features.SavedAnalyses.Commands.CreateSavedAnalysis;
using InsightHub.Application.Features.SavedAnalyses.Commands.DeleteSavedAnalysis;
using InsightHub.Application.Features.SavedAnalyses.Queries.GetUserSavedAnalyses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InsightHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SavedAnalysisController : ControllerBase
{
    private readonly IMediator _mediator;

    public SavedAnalysisController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : Guid.Empty;
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    [HttpGet]
    public async Task<IActionResult> GetUserSavedAnalyses()
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new GetUserSavedAnalysesQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new InsightHub.Application.Features.SavedAnalyses.Queries.GetSavedAnalysisById.GetSavedAnalysisByIdQuery(id, userId, IsAdmin()));
        if (result == null) return NotFound("Kaydedilmiş analiz bulunamadı.");
        return Ok(result);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id)
    {
        var userId = GetUserId();
        try
        {
            var pdfBytes = await _mediator.Send(new InsightHub.Application.Features.SavedAnalyses.Queries.ExportSavedAnalysisPdf.ExportSavedAnalysisPdfQuery(id, userId, IsAdmin()));
            return File(pdfBytes, "application/pdf", $"InsightHub_Analiz_Raporu_{id.ToString()[..8]}.pdf");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Kaydedilmiş analiz bulunamadı.");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateSavedAnalysis([FromBody] CreateSavedAnalysisRequest request)
    {
        var userId = GetUserId();
        var result = await _mediator.Send(new CreateSavedAnalysisCommand(
            userId,
            request.DatasetId,
            request.Title,
            request.Notes,
            request.AnalysisType,
            request.FilterJson,
            request.ConfigurationJson,
            request.ResultJson
        ));
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSavedAnalysis(Guid id)
    {
        var userId = GetUserId();
        var success = await _mediator.Send(new DeleteSavedAnalysisCommand(id, userId));
        if (!success)
        {
            return NotFound("Kaydedilmiş analiz bulunamadı veya silme yetkiniz yok.");
        }
        return Ok(new { Message = "Kaydedilmiş analiz başarıyla silindi." });
    }
}

public record CreateSavedAnalysisRequest(
    Guid DatasetId,
    string Title,
    string Notes,
    string AnalysisType,
    string FilterJson,
    string ConfigurationJson,
    string ResultJson
);
