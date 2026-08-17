using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Features.SavedAnalyses.Commands.CreateSavedAnalysis;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.SavedAnalyses.Queries.GetUserSavedAnalyses;

public record GetUserSavedAnalysesQuery(Guid UserId) : IRequest<List<SavedAnalysisDto>>;

public class GetUserSavedAnalysesQueryHandler : IRequestHandler<GetUserSavedAnalysesQuery, List<SavedAnalysisDto>>
{
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public GetUserSavedAnalysesQueryHandler(ISavedAnalysisRepository savedAnalysisRepository)
    {
        _savedAnalysisRepository = savedAnalysisRepository;
    }

    public async Task<List<SavedAnalysisDto>> Handle(GetUserSavedAnalysesQuery request, CancellationToken cancellationToken)
    {
        var list = await _savedAnalysisRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        return list.Select(x => new SavedAnalysisDto(
            x.Id,
            x.UserId,
            x.DatasetId,
            x.Dataset?.Name ?? "Bilinmeyen Veri Seti",
            x.Title,
            x.Notes,
            x.AnalysisType,
            x.FilterJson,
            x.ConfigurationJson,
            x.ResultJson,
            x.CreatedDate
        )).ToList();
    }
}
