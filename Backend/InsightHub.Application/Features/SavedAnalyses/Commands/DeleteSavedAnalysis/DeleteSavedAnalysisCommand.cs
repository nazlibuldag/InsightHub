using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using MediatR;

namespace InsightHub.Application.Features.SavedAnalyses.Commands.DeleteSavedAnalysis;

public record DeleteSavedAnalysisCommand(Guid Id, Guid UserId) : IRequest<bool>;

public class DeleteSavedAnalysisCommandHandler : IRequestHandler<DeleteSavedAnalysisCommand, bool>
{
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public DeleteSavedAnalysisCommandHandler(ISavedAnalysisRepository savedAnalysisRepository)
    {
        _savedAnalysisRepository = savedAnalysisRepository;
    }

    public async Task<bool> Handle(DeleteSavedAnalysisCommand request, CancellationToken cancellationToken)
    {
        var item = await _savedAnalysisRepository.GetByIdAsync(request.Id, cancellationToken);
        if (item == null || item.UserId != request.UserId)
        {
            return false;
        }

        await _savedAnalysisRepository.DeleteAsync(item, cancellationToken);
        return true;
    }
}
