using System;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using InsightHub.Domain.Entities;
using MediatR;

namespace InsightHub.Application.Features.SavedAnalyses.Commands.CreateSavedAnalysis;

public record CreateSavedAnalysisCommand(
    Guid UserId,
    Guid DatasetId,
    string Title,
    string Notes,
    string AnalysisType,
    string FilterJson,
    string ConfigurationJson,
    string ResultJson
) : IRequest<SavedAnalysisDto>;

public record SavedAnalysisDto(
    Guid Id,
    Guid UserId,
    Guid DatasetId,
    string DatasetName,
    string Title,
    string Notes,
    string AnalysisType,
    string FilterJson,
    string ConfigurationJson,
    string ResultJson,
    DateTime CreatedDate
);

public class CreateSavedAnalysisCommandHandler : IRequestHandler<CreateSavedAnalysisCommand, SavedAnalysisDto>
{
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;
    private readonly IDatasetRepository _datasetRepository;

    public CreateSavedAnalysisCommandHandler(
        ISavedAnalysisRepository savedAnalysisRepository,
        IDatasetRepository datasetRepository)
    {
        _savedAnalysisRepository = savedAnalysisRepository;
        _datasetRepository = datasetRepository;
    }

    public async Task<SavedAnalysisDto> Handle(CreateSavedAnalysisCommand request, CancellationToken cancellationToken)
    {
        var dataset = await _datasetRepository.GetByIdAsync(request.DatasetId, cancellationToken);
        if (dataset == null)
        {
            throw new Exception("Veri seti bulunamadı.");
        }

        var savedAnalysis = new SavedAnalysis
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            DatasetId = request.DatasetId,
            Title = string.IsNullOrWhiteSpace(request.Title) ? $"{dataset.Name} - Özel Analiz" : request.Title,
            Notes = request.Notes ?? string.Empty,
            AnalysisType = string.IsNullOrWhiteSpace(request.AnalysisType) ? "General" : request.AnalysisType,
            FilterJson = string.IsNullOrWhiteSpace(request.FilterJson) ? "{}" : request.FilterJson,
            ConfigurationJson = string.IsNullOrWhiteSpace(request.ConfigurationJson) ? "{}" : request.ConfigurationJson,
            ResultJson = string.IsNullOrWhiteSpace(request.ResultJson) ? "{}" : request.ResultJson,
            CreatedDate = DateTime.UtcNow
        };

        var result = await _savedAnalysisRepository.AddAsync(savedAnalysis, cancellationToken);

        return new SavedAnalysisDto(
            result.Id,
            result.UserId,
            result.DatasetId,
            dataset.Name,
            result.Title,
            result.Notes,
            result.AnalysisType,
            result.FilterJson,
            result.ConfigurationJson,
            result.ResultJson,
            result.CreatedDate
        );
    }
}
