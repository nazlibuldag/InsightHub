using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.SavedAnalyses.Queries.ExportSavedAnalysisPdf;

public record ExportSavedAnalysisPdfQuery(Guid SavedAnalysisId, Guid? UserId, bool IsAdmin) : IRequest<byte[]>;

public class ExportSavedAnalysisPdfQueryHandler : IRequestHandler<ExportSavedAnalysisPdfQuery, byte[]>
{
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;
    private readonly IPdfReportService _pdfReportService;

    public ExportSavedAnalysisPdfQueryHandler(
        ISavedAnalysisRepository savedAnalysisRepository,
        IPdfReportService pdfReportService)
    {
        _savedAnalysisRepository = savedAnalysisRepository;
        _pdfReportService = pdfReportService;
    }

    public async Task<byte[]> Handle(ExportSavedAnalysisPdfQuery request, CancellationToken cancellationToken)
    {
        var analysis = await _savedAnalysisRepository.GetByIdAsync(request.SavedAnalysisId, cancellationToken);
        if (analysis == null)
        {
            throw new KeyNotFoundException("Kaydedilmiş analiz bulunamadı.");
        }

        if (!request.IsAdmin && request.UserId.HasValue && analysis.UserId != request.UserId.Value)
        {
            throw new UnauthorizedAccessException("Bu analizin PDF raporuna erişim yetkiniz bulunmuyor.");
        }

        return await _pdfReportService.GenerateSavedAnalysisPdfReportAsync(request.SavedAnalysisId, cancellationToken);
    }
}
