using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.SavedAnalyses.Commands.CreateSavedAnalysis;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.SavedAnalyses.Queries.GetSavedAnalysisById;

public record GetSavedAnalysisByIdQuery(Guid Id, Guid? UserId, bool IsAdmin) : IRequest<SavedAnalysisDto?>;

public class GetSavedAnalysisByIdQueryHandler : IRequestHandler<GetSavedAnalysisByIdQuery, SavedAnalysisDto?>
{
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public GetSavedAnalysisByIdQueryHandler(ISavedAnalysisRepository savedAnalysisRepository)
    {
        _savedAnalysisRepository = savedAnalysisRepository;
    }

    public async Task<SavedAnalysisDto?> Handle(GetSavedAnalysisByIdQuery request, CancellationToken cancellationToken)
    {
        var analysis = await _savedAnalysisRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (analysis == null) return null;

        // Security check: Must belong to user or requester is admin
        if (!request.IsAdmin && request.UserId.HasValue && analysis.UserId != request.UserId.Value)
        {
            throw new UnauthorizedAccessException("Bu analize erişim yetkiniz bulunmuyor.");
        }

        return new SavedAnalysisDto(
            analysis.Id,
            analysis.UserId,
            analysis.DatasetId,
            analysis.Dataset?.Name ?? "Bilinmeyen Veri Seti",
            analysis.Title,
            analysis.Notes,
            analysis.AnalysisType,
            analysis.FilterJson,
            analysis.ConfigurationJson,
            analysis.ResultJson,
            analysis.CreatedDate
        );
    }
}
